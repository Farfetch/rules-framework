USE master;
IF (NOT EXISTS (SELECT 1 
                 FROM sys.databases 
                 WHERE name = '@dbname'))
BEGIN
    CREATE DATABASE [@dbname];
END