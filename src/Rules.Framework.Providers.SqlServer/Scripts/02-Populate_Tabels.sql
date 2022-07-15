USE [rules-framework-sample]

IF (NOT EXISTS (SELECT * 
                 FROM [dbo].[ConditionNodeTypes] 
                 WHERE [Code] IN (1, 2, 3, 4, 5, 6, 7)))
BEGIN

INSERT INTO [dbo].[ConditionNodeTypes] 
    VALUES
    (1, 'Boolean'),
    (2, 'Composed'),
    (3, 'Decimal'),
    (4, 'Integer'),
    (5, 'String'),
    (7, 'Value')

END


IF (NOT EXISTS (SELECT * 
                 FROM [dbo].[DataTypes] 
                 WHERE [Code] IN (1, 2, 3, 4)))
BEGIN

INSERT INTO [dbo].[DataTypes] 
    VALUES
    (1, 'Integer'),
    (2, 'Decimal'),
    (3, 'String'),
    (4, 'Boolean')

END


IF (NOT EXISTS (SELECT * 
                 FROM [dbo].[Operators] 
                 WHERE [Code] IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)))
BEGIN

INSERT INTO [dbo].[Operators] 
    VALUES
    (1, 'Equal', '='),
    (2, 'NotEqual', '<>'),
    (3, 'GreaterThan', '>'),
    (4, 'GreaterThanOrEqual', '>='),
    (5, 'LesserThan', '<'),
    (6, 'LesserThanOrEqual', '=<'),
    (7, 'Contains', 'CONTAINS'),
    (8, 'In', 'IN'),
    (9, 'StartsWith', 'STARTSWITH'),
    (10, 'EndsWith', 'ENDSWITH'),
    (11, 'CaseInsensitiveStartsWith', 'CASEINSENSITIVESTARTSWITH'),
    (12, 'CaseInsensitiveEndsWith', 'CASEINSENSITIVEENDSWITH'),
    (13, 'And', 'AND'),
    (14, 'Or', 'OR'),
    (15, 'Eval', 'EVAL')

END 