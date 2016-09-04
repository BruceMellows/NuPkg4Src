NuPkg4Src
=========

This tool to simplify creating source distributed using NuGet

## TLDR;

* Create a $(SolutionDir)\Media folder.
* Copy your favorite nuget.exe to the $(SolutionDir)\Media folder (I successfully have used 3.4.4.1321) - the Example Library expects to find it there.
* Compile and observe nuget packages are created in the Media folder.

## A little more detail please

Please observe the first few lines in the ExampleLibrary sources, they control what files are converted and how they are converted into NuPkg files.

Create your source files and add the "// NuPkg4Src-..." stuff from the *very* first line in the source, the first line that is not decorated as such will be treated as the beginning of the files content.

The NuPkg4Src-... stuff loosely corresponds to the metadata in the NuSpec file.

If the source file is missing the hash or the version, theses are added.

If the source file hash does not match the content, the version is updated (+1 build) and the new hash is written.

## Todo

* Proper Documentation
