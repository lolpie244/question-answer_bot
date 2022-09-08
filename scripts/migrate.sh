cd ../
echo "Write migration name"
read migration_name

echo "
====================
Create migration
====================
"
dotnet ef migrations add $migration_name --context dbContext
echo "
====================
Update db
====================
"
dotnet ef database update --context dbContext
echo "
====================
Optimize models
====================
"
dotnet ef dbContext optimize --context dbContext -o db/CompiledModels -n db_namespace