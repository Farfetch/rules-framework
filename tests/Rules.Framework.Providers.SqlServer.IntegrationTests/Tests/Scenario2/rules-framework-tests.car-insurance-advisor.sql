
insert into ContentTypes values (1, 'CarInsuranceAdvice')

insert into ConditionTypes values (1, 'RepairCostsCommercialValueRate') 

insert into ConditionNodes ([Id], [ConditionNodeTypeCode], [ConditionTypeCode], [DataTypeCode], [OperatorCode], [Operand])
values (1, ,1, 2, 15, '80')
insert into ConditionNodes ([Id], [ConditionNodeTypeCode], [ConditionTypeCode], [DataTypeCode], [OperatorCode], [Operand])
values (2, ,1, 2, 15, '80')

insert into Rules
values  (1, 'Pay', 1, '2018-01-01T00:00:00Z', null, 'Car Insurance Advise on repair costs lesser than 80% of commercial value', 1, 1)

insert into Rules
values  (2, 'PayNewCar', 1, '2018-01-01T00:00:00Z', null, 'Car Insurance Advise on repair costs greater than 80% of commercial value', 2, 2)


