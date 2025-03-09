# Sample ATM App with Microsoft Orleans

This repository contains code for a simple (really) ATM application that uses
Microsoft Orleans to demonstrate the Actor Pattern.

## Starting

Instructions below will show your the way to get things working.

Please give a thumbup to this guy here [jumpstartCS](https://www.youtube.com/@jumpstartCS).
He is responsible for what you see in this repository. Also see his video series on the topic: [About Microsoft Orleans](https://www.youtube.com/watch?v=JmJxi6hEA-w&list=PLalrWAGybpB9mEE84ybHSCxUxPq9ToaZE)

### Pre-requirements

If you have a Microsoft Azure subscription you can use it or if you more like
me, a broke-ass developer, you can run Azurite on Docker and get it going:

```powershell
docker run -name azurite -d -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```
You can get more information from Microsoft at [Use the Azurite emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage)

## Coding Styles

Nothing written into stone, use your ol'good common sense. But you can refere
to this page, if you like: [Common C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

## Contribuition

Just me, at the moment.

## Authors

- **Marco Teixeira (marcoaoteixeira)** - _initial work_

## License

MIT

## Acknowledgement

- Hat tip to anyone whose code was used.