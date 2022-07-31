using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using PixiEditor.Parser.Benchmarks;
//
// var marks = new Benchmarks();
//
// marks.Setup();

BenchmarkRunner.Run<Benchmarks>();
