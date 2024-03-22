--USE  [@dbname]

--IF NOT EXISTS ( SELECT  *
--                FROM    sys.schemas
--                WHERE   name = N'@schemaname' )
--BEGIN
--	EXEC('CREATE SCHEMA '+ @schemaname)
--END

USE  [@dbname]

IF NOT EXISTS ( SELECT  *
                FROM    sys.schemas
                WHERE   name = N'@schemaname' )
BEGIN
    EXEC('CREATE SCHEMA [@schemaname]');
END