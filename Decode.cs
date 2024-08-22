// Copyright (c) Team CharLS.
// SPDX-License-Identifier: BSD-3-Clause

using BenchmarkDotNet.Attributes;

namespace JpegLS.Benchmark;

public class Decode
{
    private byte[]? _source;
    private byte[]? _destination;

    // Keep warm copies of the decoders (read-only tables are cached between instances).
    // ReSharper disable UnusedMember.Local
    private CharLS.Native.JpegLSDecoder _decoderKeepNative = new();
    private CharLS.Managed.JpegLSDecoder _decoderKeepManaged = new();
    // ReSharper restore UnusedMember.Local

    [GlobalSetup]
    public void GlobalSetup()
    {
        _source = File.ReadAllBytes("d:/benchmark-test-image.jls");

        var decoder = new CharLS.Managed.JpegLSDecoder(_source);
        _destination = new byte[decoder.GetDestinationSize()];
    }

    [Benchmark]
    public void DecodeCharLSManaged()
    {
        var decoder = new CharLS.Managed.JpegLSDecoder(_source);

        decoder.Decode(_destination);
    }

    [Benchmark]
    public void DecodeCharLSNative()
    {
        using var decoder = new CharLS.Native.JpegLSDecoder(_source);

        decoder.Decode(_destination);
    }

    [Benchmark]
    public void DecodeCSCharls()
    {
        CharLS.JpegLs.ReadHeader(_source, out var parameters, out string _);
        CharLS.JpegLs.Decode(_destination, _source, parameters, out string _);
    }
}
