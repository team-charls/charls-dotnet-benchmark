// See https://aka.ms/new-console-template for more information


using System;
using System.Reflection;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;

namespace MyBenchmarks
{
    public class JpegLSDecoders
    {
        private readonly byte[] source;
        private readonly byte[] destination;


        public JpegLSDecoders()
        {
            source = File.ReadAllBytes("d:/artificial-none.jls");

            using var decoder = new CharLS.Native.JpegLSDecoder(source);
            destination = new byte[decoder.GetDestinationSize()];
        }

        [Benchmark]
        public byte[] DecodeCharLSNative()
        {
            using var decoder = new CharLS.Native.JpegLSDecoder(source);

            decoder.Decode(destination);

            return destination;
        }

        [Benchmark]
        public byte[] DecodeCSCharls()
        {
            CharLS.JpegLs.ReadHeader(source, out var parameters, out string _);
            CharLS.JpegLs.Decode(destination, source, parameters, out string _);

            return destination;
        }

        private static string DataFileDirectory
        {
            get
            {
                var assemblyLocation = new Uri(Assembly.GetExecutingAssembly().Location);
                return Path.GetDirectoryName(assemblyLocation.LocalPath)!;
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}