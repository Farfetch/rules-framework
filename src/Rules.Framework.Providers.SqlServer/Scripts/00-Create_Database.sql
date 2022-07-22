USE master;
IF (NOT EXISTS (SELECT * 
                 FROM sys.databases 
                 WHERE name = 'rules-framework-sample'))
BEGIN
    CREATE DATABASE [rules-framework-sample];
END