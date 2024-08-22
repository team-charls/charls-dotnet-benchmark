// Copyright (c) Team CharLS.
// SPDX-License-Identifier: BSD-3-Clause

using BenchmarkDotNet.Attributes;

namespace JpegLS.Benchmark;

public class EncodeMonochromeLossless
{
    private byte[]? _source;
    private byte[]? _destination;
    private PortableAnymapFile? _referenceFile;

    // Keep warm copies of the encoders (read-only tables are cached between instances).
    public readonly CharLS.Native.JpegLSEncoder _encoderKeepNative = new();
    public readonly CharLS.Managed.JpegLSEncoder _encoderKeepManaged = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        _referenceFile = new PortableAnymapFile("d:/benchmark-test-image.pgm");

        _source = _referenceFile.ImageData;

        var encoder = new CharLS.Managed.JpegLSEncoder(
            _referenceFile.Width, _referenceFile.Height, _referenceFile.BitsPerSample, _referenceFile.ComponentCount);
        _destination = new byte[encoder.EstimatedDestinationSize];
    }

    [Benchmark]
    public void EncodeCharLSManaged()
    {
        var encoder = new CharLS.Managed.JpegLSEncoder(
            _referenceFile!.Width, _referenceFile.Height, _referenceFile.BitsPerSample, _referenceFile.ComponentCount, CharLS.Managed.InterleaveMode.None, false)
        {
            Destination = _destination
        };

        encoder.Encode(_source);
    }

    [Benchmark]
    public void EncodeCharLSNative()
    {
        using var encoder = new CharLS.Native.JpegLSEncoder(
            _referenceFile!.Width, _referenceFile.Height, _referenceFile.BitsPerSample, _referenceFile.ComponentCount, false);

        encoder.Destination = _destination;
        encoder.Encode(_source);
    }

    [Benchmark]
    public void EncodeCSCharls()
    {
        CharLS.JlsParameters jlsParameters = new()
        {
            bitsPerSample = _referenceFile!.BitsPerSample,
            components = _referenceFile!.ComponentCount,
            width = _referenceFile!.Width,
            height = _referenceFile!.Height
        };

        _ = CharLS.JpegLs.Encode(_destination, _source, jlsParameters, out _, out _);
    }
}
