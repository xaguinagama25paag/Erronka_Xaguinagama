using Erronka_XabierAguinagaMarin_Zerbitzari;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Xunit;

public class DatubaseaTests
{
    string CreateTempDbWithSchema()
    {
        var tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".s3db");
        SQLiteConnection.CreateFile(tmp);
        using var conn = new SQLiteConnection($"Data Source={tmp}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE Erabiltzaileak (id INTEGER PRIMARY KEY AUTOINCREMENT, izena TEXT, pasahitza TEXT);
            CREATE TABLE Puntuazioa (id INTEGER PRIMARY KEY AUTOINCREMENT, idErabiltzaile INTEGER, puntuazio INTEGER, idEbaluatzaile INTEGER);
        ";
        cmd.ExecuteNonQuery();
        return tmp;
    }

    void SetupDatabConnection(string dbFile)
    {
        var conn = new SQLiteConnection($"Data Source={dbFile}");
        var field = typeof(Datubasea).GetField("dbConnection", BindingFlags.NonPublic | BindingFlags.Static);
        field.SetValue(null, conn);
        Datubasea.Konektatu();
    }

    [Fact]
    public void SortuKontua_Then_LortuErabiltzailea_ReturnsCreatedUser()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            Datubasea.Konektatu();

            datab.sortuKontua("probaUser", "Pasahitz1");
            var user = datab.lortuErabiltzailea("probaUser", "Pasahitz1");

            Assert.NotNull(user);
            Assert.True(user.getId() > 0);
            Assert.Equal("probaUser", user.getIzena());
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void ErabiltzaileExistitzen_BeforeAndAfterCreation_ReturnsExpectedValues()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();

            var before = datab.erabiltzaileExistitzen("uniqUser");
            Assert.Equal("true", before);

            datab.sortuKontua("uniqUser", "pwd");
            var after = datab.erabiltzaileExistitzen("uniqUser");
            Assert.Equal("false", after);
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void LortuErabiltzaile_ById_ReturnsCorrectName()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            datab.sortuKontua("byIdUser", "pwd");
            var user = datab.lortuErabiltzailea("byIdUser", "pwd");
            var name = datab.lortuErabiltzaile(user.getId());
            Assert.Equal("byIdUser", name);
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void LortuPuntuazioak_NoScores_ReturnsListWithZero()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            datab.sortuKontua("scoreless", "pwd");
            var user = datab.lortuErabiltzailea("scoreless", "pwd");
            var scores = datab.LortuPuntuazioak(user.getId());
            Assert.Single(scores);
            Assert.Equal(0, scores[0]);
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void SortuRating_Then_LortuPuntuazioak_ReturnsRating()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            datab.sortuKontua("rated", "pwd");
            datab.sortuKontua("rater", "pwd");
            var rated = datab.lortuErabiltzailea("rated", "pwd");
            var rater = datab.lortuErabiltzailea("rater", "pwd");

            datab.sortuRating(5, rater.getId(), rated.getId());

            var scores = datab.LortuPuntuazioak(rated.getId());
            Assert.Contains(5, scores);
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void LortuEmandakoPuntuazioak_NoScores_ReturnsListWithZero()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            datab.sortuKontua("noGiven", "pwd");
            var user = datab.lortuErabiltzailea("noGiven", "pwd");
            var given = datab.LortuEmandakoPuntuazioak(user.getId());
            Assert.Single(given);
            Assert.Equal(0, given[0]);
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void LortuEmandakoPuntuazioak_ReturnsRating_WhenRatingCreated()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            datab.sortuKontua("t1", "pwd");
            datab.sortuKontua("t2", "pwd");
            var t1 = datab.lortuErabiltzailea("t1", "pwd");
            var t2 = datab.lortuErabiltzailea("t2", "pwd");
            datab.sortuRating(3, t1.getId(), t2.getId());

            var given = datab.LortuEmandakoPuntuazioak(t1.getId());
            Assert.Contains(3, given);
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void MultipleRatings_ReturnsAllRatings()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            datab.sortuKontua("userA", "pwd");
            datab.sortuKontua("r1", "pwd");
            datab.sortuKontua("r2", "pwd");
            var userA = datab.lortuErabiltzailea("userA", "pwd");
            var r1 = datab.lortuErabiltzailea("r1", "pwd");
            var r2 = datab.lortuErabiltzailea("r2", "pwd");

            datab.sortuRating(2, r1.getId(), userA.getId());
            datab.sortuRating(4, r2.getId(), userA.getId());

            var scores = datab.LortuPuntuazioak(userA.getId());
            Assert.Contains(2, scores);
            Assert.Contains(4, scores);
            Assert.True(scores.Count >= 2);
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void LortuErabiltzailea_WrongCredentials_ReturnsNotFound()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            datab.sortuKontua("exist", "pwd");
            var user = datab.lortuErabiltzailea("exist", "wrongpwd");
            Assert.Equal(-1, user.getId());
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }

    [Fact]
    public void DuplicateUser_CreatesMultipleRecords()
    {
        var dbFile = CreateTempDbWithSchema();
        try
        {
            SetupDatabConnection(dbFile);
            var datab = new Datubasea();
            datab.sortuKontua("dupUser", "pwd1");
            datab.sortuKontua("dupUser", "pwd2");

            var field = typeof(Datubasea).GetField("dbConnection", BindingFlags.NonPublic | BindingFlags.Static);
            var conn = (SQLiteConnection)field.GetValue(null);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Erabiltzaileak WHERE izena = 'dupUser';";
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            Assert.Equal(2, count);
        }
        finally
        {
            Datubasea.Itzali();
            File.Delete(dbFile);
        }
    }
}