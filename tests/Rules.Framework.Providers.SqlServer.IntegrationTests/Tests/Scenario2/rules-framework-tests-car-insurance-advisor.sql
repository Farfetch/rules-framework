use [@dbname]

insert into ContentTypes values (1, 'CarInsuranceAdvice')

insert into ConditionTypes values (1, 'RepairCosts') 
insert into ConditionTypes values (2, 'RepairCostsCommercialValueRate') 


--Rule 1

insert into ConditionNodes ([ConditionNodeTypeCode], [ConditionTypeCode], [DataTypeCode], [OperatorCode], [Operand], [LogicalOperatorCode])
values (1, 2, 2, 5, '80', 3)

DECLARE @ConditionNodeId AS BIGINT = (SELECT SCOPE_IDENTITY())

insert into Rules ([Content], [ContentTypeCode], [DateBegin], [DateEnd], [Name], [Priority], [ConditionNodeId])
values  ('Pay', 1, '2018-01-01T00:00:00Z', null, 'Car Insurance Advise on repair costs lesser than 80% of commercial value', 1, @ConditionNodeId)

--Rule 2

insert into ConditionNodes ([ConditionNodeTypeCode], [ConditionTypeCode], [DataTypeCode], [OperatorCode], [Operand], [LogicalOperatorCode])
values (1, 2, 2, 4, '80', 3)

SET @ConditionNodeId = (SELECT SCOPE_IDENTITY())

insert into Rules ([Content], [ContentTypeCode], [DateBegin], [DateEnd], [Name], [Priority], [ConditionNodeId])
values  ('PayNewCar', 1, '2018-01-01T00:00:00Z', null, 'Car Insurance Advise on repair costs greater than 80% of commercial value', 2, @ConditionNodeId)

--Rule3

insert into ConditionNodes ([ConditionNodeTypeCode], [ConditionTypeCode], [DataTypeCode], [OperatorCode], [Operand], [LogicalOperatorCode])
values (2, 1, 2, null, null, 1)

DECLARE @ParentNodeId AS BIGINT = (SELECT SCOPE_IDENTITY())

insert into ConditionNodes ([ConditionNodeTypeCode], [ConditionTypeCode], [DataTypeCode], [OperatorCode], [Operand], [LogicalOperatorCode])
values (1, 1, 2, 5, '1000', 3)

DECLARE @Child1NodeId AS BIGINT = (SELECT SCOPE_IDENTITY())

insert into ConditionNodes ([ConditionNodeTypeCode], [ConditionTypeCode], [DataTypeCode], [OperatorCode], [Operand], [LogicalOperatorCode])
values (1, 2, 2, 5, '80', 3)

DECLARE @Child2NodeId AS BIGINT = (SELECT SCOPE_IDENTITY())

INSERT INTO ConditionNodeRelations ([OwnerId], [ChildId]) VALUES (@ParentNodeId, @Child1NodeId)
INSERT INTO ConditionNodeRelations ([OwnerId], [ChildId]) VALUES (@ParentNodeId, @Child2NodeId)

insert into Rules ([Content], [ContentTypeCode], [DateBegin], [DateEnd], [Name], [Priority], [ConditionNodeId])
values  ('RefusePaymentPerFranchise', 1, '2018-01-01T00:00:00Z', null, 'Car Insurance Advise on repair costs lower than franchise boundary', 3, @ParentNodeId)

