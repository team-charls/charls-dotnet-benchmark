// Copyright (c) Team CharLS.
// SPDX-License-Identifier: BSD-3-Clause

using BenchmarkDotNet.Attributes;
using CharLS.Managed;

namespace JpegLS.Benchmark;

public class EncodeMonochromeLossless
{
    private byte[]? _source;
    private byte[]? _destination;
    private PortableAnymapFile? _referenceFile;

    // Keep warm copies of the encoders (read-only tables are cached between instances).
    private CharLS.Native.JpegLSEncoder _encoderKeepNative = new CharLS.Native.JpegLSEncoder();
    private CharLS.Managed.JpegLSEncoder _encoderKeepManaged = new CharLS.Managed.JpegLSEncoder();

    [GlobalSetup]
    public void GlobalSetup()
    {
        _referenceFile = new("d:/benchmark-test-image.pgm");

        _source = _referenceFile.ImageData;

        var encoder = 
            new CharLS.Managed.JpegLSEncoder(_referenceFile.Width, _referenceFile.Height, _referenceFile.BitsPerSample, _referenceFile.ComponentCount);
        _destination = new byte[encoder.EstimatedDestinationSize];
    }

    [Benchmark]
    public void EncodeCharLSManaged()
    {
        var encoder = new CharLS.Managed.JpegLSEncoder(
            _referenceFile!.Width, _referenceFile.Height, _referenceFile.BitsPerSample, _referenceFile.ComponentCount, InterleaveMode.None, false)
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
        var jlsParameters = new CharLS.JlsParameters();
        jlsParameters.bitsPerSample = _referenceFile!.BitsPerSample;
        jlsParameters.components = _referenceFile!.ComponentCount;
        jlsParameters.width = _referenceFile!.Width;
        jlsParameters.height = _referenceFile!.Height;

        CharLS.JpegLs.Encode(_destination, _source, jlsParameters, out var _, out var _);
    }
}
