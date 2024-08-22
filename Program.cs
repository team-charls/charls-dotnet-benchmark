// Copyright (c) Team CharLS.
// SPDX-License-Identifier: BSD-3-Clause

using BenchmarkDotNet.Running;

namespace JpegLS.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args);
    }
}
