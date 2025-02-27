# Nameless

This repository contains all code that I assume is useful in most of the cases
where I need to build something. So, if you think that it could be useful for
you, feel free to fork, clone, etc. Also, I tried to mention every person that
I got something from. If you find code that needs to be given the correct
authorship, please, let me know.

## Starting

Instructions below will show your the way to get things working.

### Pre-requirements

You'll need to install, at least, .NET Core SDK and Visual Studio Code.
Just follow the links:

- [.NET Core SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
- [Visual Studio Code](https://code.visualstudio.com/download)

## Development

Here are a couple of apps that I find useful when developing:

- [LINQPad](https://www.linqpad.net/download.aspx): For writing LINQ-Queries or
use as a REPL.
- [Notepad++](https://notepad-plus-plus.org/downloads/): You know what this is,
don't you?
- [SQLite Browser](https://sqlitebrowser.org/dl/): One of the best data browsers
for SQLite databases.
- [Git](https://git-scm.com/downloads): No need (I hope) for presentations.
- [ILSpy](https://github.com/icsharpcode/ILSpy/releases): When you want to take
a peek on the compiled code that no one have the source.
- [SMTP4Dev](https://github.com/rnwood/smtp4dev/releases): Small SMTP server
that runs as a service on your machine, for send e-mail tests. Very useful.
- [DBeaver](https://dbeaver.io/download/): An alternative for databases
management. Don't have all options for each specific database, but, Oh-boy,
it's useful.
- [Postman](https://www.postman.com/downloads/): Useful to test Rest API calls.
Similar to SoapUI, but I think that it's much more easy to handle.
- [Sonarlint](https://www.sonarsource.com/products/sonarlint/): Analyze and 
suggest code modifications on-the-fly. Has a free-tier.

These are all just suggestions.


## Testing

Maybe you'll need to install the coverage tool and report tool. If I'm not
mistaken, Visual Studio already has those dependencies installed for you after
restore. But...

_.NET Coverlet Tool_

```
dotnet tool install -g coverlet.console
```

_.NET Report Generator Tool_

```
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Skipping Tests

Some tests will need a software to run or any other resource that may not be available on your
build environment. For example, ProducerConsumer for RabbitMQ project needs RabbitMQ to run.

For these tests to run on my GitHub Actions (workflow) I needed a way to "mute" them and run only
locally, since I could setup, again, RabbitMQ on a Docker container on my machine.

So, if you need to "mute" a test and run it only locally, use the **RunsOnDevMachineAttribute** attribute
on your test method.

### Creating Code Coverage Report

When you feel comfortable, you should run all tests and output the coverage
result to a folder. The Report Generator Tool will take from there. So, do as
follow:

Run tests with:
```
dotnet test --logger:"Html;LogFileName=code-coverage-log.html" --collect:"XPlat Code Coverage" --results-directory ./code-coverage/ --verbosity normal
```

Run the report tool with:
```
reportgenerator "-reports:./code-coverage/**/coverage.cobertura.xml" "-targetdir:./code-coverage/report" -reporttypes:Html
```

If you want to know more about the report tool, see their [FAQ](https://reportgenerator.io/usage).

## Coding Styles

Nothing written into stone, use your ol'good common sense. But you can refere
to this page, if you like: [Common C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

### Some Ideas to Keep in Mind

- When I create a new extension method I never check for nullability of the
  target instance. If the target instance is **_null_**, you'll get
  an **_NullReferenceException_** on your face, punk!
- **_default_** vs **_null_** :
  - Am I sure is a reference type? **null**
  - Am I sure is a value type? **default**
  - I'm not sure at all: **default**
- I think that most of my developer's life I spent writing C# code. But I like
  that kind of code organization from Java. So, I use, almost, the same code
  style identation on my projects.
- Methods that returns arrays, collections or enumerables in general,
  **DO NOT RETURN _NULL_ VALUE EVER!!!** If there's no value to return, just
  return an empty enumerable from the same type. Use [_Array.Empty\<T\>()_](https://learn.microsoft.com/en-us/dotnet/api/system.array.empty?view=net-7.0) or
  [_Enumerable.Empty\<T\>()_](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.empty?view=net-7.0),
  or I'll cut the tips of your fingers with a rust scissor...thanks.
  - If you're returning an **_IEnumerable\<T\>_**, probably you should use the
  **_yield_** keywork on your method return directive.

### Dependency Injection "Battle"

Let us not talk about [Unity](https://learn.microsoft.com/en-us/previous-versions/msp-n-p/ff647202(v=pandp.10)?redirectedfrom=MSDN)
or [MEF](https://learn.microsoft.com/en-us/dotnet/framework/mef/). These were from
a really long time ago, on a far-far away galaxy...(I think that MEF is still in use
inside some modern projects from Microsoft).

Look, I, really, understand that [Microsoft](https://www.microsoft.com/pt-br/)
has its own modern dependency injection system ([ServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollection) extensions).
It'll work for most of all simple cases and applications. But for more complex
scenarios, some that I cross paths on my developer's life, it'll not be the best.

Don't get me wrong, it's simple, fast, reliable and easy to use. And should be
your first choice if you're starting a new ASP.NET Core project.

But I'll advocate for [Autofac](https://autofac.readthedocs.io/en/latest/index.html).
It has way more road beneath its codebase and has some amazing features. Like
[Decorators](https://autofac.readthedocs.io/en/latest/advanced/adapters-decorators.html) and
[Type Interceptors](https://autofac.readthedocs.io/en/latest/advanced/interceptors.html).
So, that said, all projects inside this repository will favor Autofac. 🤘😜

## Deployment

I'm using GitHub Actions to act as a CI/CD. All files are located in the
.github folder.

## Template

There is a small "microservice" template inside the template folder. To
install it just use:

```
dotnet new install <FULL_PATH_TO_MICROSERVICE_FOLDER>
```

E.g:

```
dotnet new install C:\Workspace\Nameless\template\Microservice\Nameless.Microservice
```

After that you can create a new project, using **dotnet new** or
**Visual Studio / New Project** and search for **_Microservice_** template.

## Contribuition

Just me, at the moment.

## Versioning

Using [SemVer](http://semver.org/) for assembly versioning.

## Authors

- **Marco Teixeira (marcoaoteixeira)** - _initial work_

## License

MIT

## Acknowledgement

- Hat tip to anyone whose code was used.