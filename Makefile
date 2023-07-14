publish:
	dotnet publish franko_bot/franko_bot.csproj -c Release

build:
	docker build --no-cache . -t franko_bot

run:
	docker run -it -p 59890:59890 franko_bot

ngrok:
	ngrok http 59890
