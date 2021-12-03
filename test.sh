#!/bin/sh
pushd Src
dotnet test --filter FullyQualifiedName~.Tests
popd
