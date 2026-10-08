using KaraokeCodingChallenges.MediaLibraryDatabase.Mapping;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Repositories;
using KaraokeCodingChallenges.ScanResult;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.MediaLibraryDatabase.Services;

public class ScannedTrackPersistenceService
{
    private readonly IMediaTrackRepository _trackRepository;
    private readonly string _connectionString;

    public ScannedTrackPersistenceService(IMediaTrackRepository trackRepository, string connectionString)
    {
        _trackRepository = trackRepository;
        _connectionString = connectionString;
    }

    public long? Persist(MediaScanResult scanResult, long sourceId, bool isKaraoke = false)
    {
        MediaTrackRecord? track = ScannedTrackMapper.Map(scanResult, sourceId, isKaraoke);

        if (track is null)
        {
            return null;
        }

        MediaTrackRecord? existingTrack = _trackRepository.GetByFilePath(track.FilePath);

        if (existingTrack is null)
        {
            return _trackRepository.Add(track);
        }

        MediaTrackRecord updatedTrack = track with
        {
            Id = existingTrack.Id
        };

        _trackRepository.Update(updatedTrack);

        return existingTrack.Id;
    }

    public IReadOnlyList<long> PersistBatch(IEnumerable<MediaScanResult> scanResults, long sourceId, bool isKaraoke = false)
    {
        List<long> persistedTrackIds = new List<long>();

        using SqliteConnection connection = new SqliteConnection(_connectionString);

        connection.Open();

        using SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            foreach (MediaScanResult scanResult in scanResults)
            {
                MediaTrackRecord? track = ScannedTrackMapper.Map(scanResult, sourceId, isKaraoke);

                if (track is null)
                {
                    continue;
                }

                MediaTrackRecord? existingTrack = _trackRepository.GetByFilePath(track.FilePath, connection, transaction);

                long trackId;

                if (existingTrack is null)
                {
                    trackId = _trackRepository.Add(track, connection, transaction);
                }
                else
                {
                    MediaTrackRecord updatedTrack = track with
                    {
                        Id = existingTrack.Id
                    };

                    _trackRepository.Update(updatedTrack, connection, transaction);

                    trackId = existingTrack.Id;
                }

                persistedTrackIds.Add(trackId);
            }

            transaction.Commit();

            return persistedTrackIds;
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }
}
