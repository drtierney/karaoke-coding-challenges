using KaraokeCodingChallenges.IncrementalScanning.Models;
using KaraokeCodingChallenges.MediaLibraryDatabase.Models;
using Microsoft.Data.Sqlite;

namespace KaraokeCodingChallenges.IncrementalScanning.Services;

public class LibraryReconciliationPersistenceService
{
    private readonly string _connectionString;

    public LibraryReconciliationPersistenceService(string connectionString)
    {
        _connectionString = connectionString;
    }


    public void Apply(LibraryReconciliationResult result)
    {
        if (!result.ScanCompletedSuccessfully)
        {
            throw new InvalidOperationException("Cannot apply reconciliation results from an unsuccessful scan.");
        }

        using SqliteConnection connection = new(_connectionString);

        connection.Open();

        using SqliteTransaction transaction = connection.BeginTransaction();

        foreach (MediaTrackRecord track in result.MissingTracks)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText = """
                UPDATE Tracks
                SET IsAvailable = 0
                WHERE Id = $trackId
                AND SourceId = $sourceId;
                """;

            command.Parameters.AddWithValue("$trackId", track.Id);
            command.Parameters.AddWithValue("$sourceId", result.SourceId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected != 1)
            {
                throw new InvalidOperationException($"Unable to reconcile track {track.Id} for source {result.SourceId}.");
            }
        }

        foreach (MediaTrackRecord track in result.PresentTracks)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText = """
                UPDATE Tracks
                SET IsAvailable = 1
                WHERE Id = $trackId
                AND SourceId = $sourceId;
                """;

            command.Parameters.AddWithValue("$trackId", track.Id);
            command.Parameters.AddWithValue("$sourceId", result.SourceId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected != 1)
            {
                throw new InvalidOperationException($"Unable to restore track {track.Id} for source {result.SourceId}.");
            }
        }


        foreach (FileMoveMatch move in result.MovedTracks)
        {
            using SqliteCommand command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText = """
                UPDATE Tracks
                SET FilePath = $newFilePath, IsAvailable = 1
                WHERE Id = $trackId
                  AND SourceId = $sourceId
                  AND FilePath = $oldFilePath COLLATE NOCASE;
                """;

            command.Parameters.AddWithValue("$newFilePath", move.NewFilePath);
            command.Parameters.AddWithValue("$trackId", move.TrackId);
            command.Parameters.AddWithValue("$sourceId", result.SourceId);
            command.Parameters.AddWithValue("$oldFilePath", move.OldFilePath);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected != 1)
            {
                throw new InvalidOperationException($"Unable to reconcile moved track {move.TrackId} for source {result.SourceId}.");
            }
        }


        transaction.Commit();
    }

}
