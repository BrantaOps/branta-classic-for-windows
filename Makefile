run:
	dotnet run --project=Branta

test:
	dotnet test

coverage:
	dotnet-coverage collect -f xml -o coverage.xml dotnet test
	reportgenerator -reports:"coverage.xml" -targetdir:"coveragereport" -reporttypes:Html
	cmd /c start ./coveragereport/index.html

coverage-setup:
	dotnet tool install --global dotnet-coverage
	dotnet tool install --global dotnet-reportgenerator-globaltool
