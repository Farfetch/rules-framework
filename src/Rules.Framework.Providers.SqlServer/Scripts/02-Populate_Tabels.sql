USE [rules-framework-sample]

IF (NOT EXISTS (SELECT * 
                 FROM [dbo].[ConditionNodeTypes] 
                 WHERE [Code] IN (1, 2)))
BEGIN

INSERT INTO [dbo].[ConditionNodeTypes] 
    VALUES
    (1, 'ValueConditionNode'),
    (2, 'ComposedConditionNode')

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
                 WHERE [Code] IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)))
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
    (12, 'CaseInsensitiveEndsWith', 'CASEINSENSITIVEENDSWITH')

END

IF (NOT EXISTS (SELECT * 
                 FROM [dbo].[LogicalOperators] 
                 WHERE [Code] IN (1, 2, 3)))
BEGIN

INSERT INTO [dbo].[LogicalOperators] 
    VALUES
    (1, 'And', 'AND'),
    (2, 'Or', 'OR'),
    (3, 'Eval', 'EVAL')

END 