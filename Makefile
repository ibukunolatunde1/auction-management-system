.PHONY: run-api run-console test test-domain test-application test-infrastructure test-with-coverage

run-api:
	dotnet run --project src/CarAuctionSystem.Api/CarAuctionSystem.Api.csproj

run-console:
	dotnet run --project src/CarAuctionSystem.Console/CarAuctionSystem.Console.csproj

test: test-domain test-application test-infrastructure

test-with-coverage:
	@rm -rf TestResults
	@mkdir -p TestResults
	dotnet test /p:CollectCoverage=true /p:CoverletOutput=$(PWD)/TestResults/ /p:CoverletOutputFormat=cobertura /p:MergeWith=$(PWD)/TestResults/coverage.json
	dotnet tool install --tool-path ./tools dotnet-reportgenerator-globaltool --version 5.1.26 || true
	./tools/reportgenerator \
		"-reports:$(PWD)/TestResults/coverage.cobertura.xml" \
		"-targetdir:TestResults/CoverageReport" \
		-reporttypes:Html

test-domain:
	dotnet test tests/CarAuctionSystem.Domain.Tests/CarAuctionSystem.Domain.Tests.csproj

test-application:
	dotnet test tests/CarAuctionSystem.Application.Tests/CarAuctionSystem.Application.Tests.csproj

test-infrastructure:
	dotnet test tests/CarAuctionSystem.Infrastructure.Tests/CarAuctionSystem.Infrastructure.Tests.csproj

clean-test:
	rm -rf TestResults/