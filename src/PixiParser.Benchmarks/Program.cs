using BenchmarkDotNet.Running;
using PixiEditor.Parser.Benchmarks.Benchmarks;

//
// var marks = new Benchmarks();
//
// marks.Setup();

BenchmarkRunner.Run<Benchmarks>();
