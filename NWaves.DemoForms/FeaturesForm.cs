﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NWaves.Audio;
using NWaves.FeatureExtractors.Base;
using NWaves.FeatureExtractors.Multi;
using NWaves.Signals;
using NWaves.Features;
using System.Drawing;
using NWaves.Transforms;

namespace NWaves.DemoForms
{
    public partial class FeaturesForm : Form
    {
        private DiscreteSignal _signal;
        private FeatureVector[] _vectors;

        public FeaturesForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (var stream = new FileStream(ofd.FileName, FileMode.Open))
            {
                var waveFile = new WaveFile(stream);
                _signal = waveFile[Channels.Left];
            }

            var frameSize = (double) 4096 / _signal.SamplingRate;
            var hopSize = (double) 2048 / _signal.SamplingRate;

            var freqs = new[] { 0.0f, 300, 600, 1000, 2000, 4000, 7000 };


            var tdExtractor = new TimeDomainFeaturesExtractor(_signal.SamplingRate, "all", frameSize, hopSize);
            var spectralExtractor = new SpectralFeaturesExtractor(_signal.SamplingRate, "all", frameSize, hopSize, frequencies: freqs);

            tdExtractor.AddFeature("pitch_zcr", (signal, start, end) => { return Pitch.FromZeroCrossingsSchmitt(signal, start, end); });
            //spectralExtractor.AddFeature("pitch_hss", (spectrum, fs) => { return Pitch.FromHss(spectrum, _signal.SamplingRate); } );

            var tdVectors = tdExtractor.ParallelComputeFrom(_signal);
            var spectralVectors = spectralExtractor.ParallelComputeFrom(_signal);

            _vectors = FeaturePostProcessing.Join(tdVectors, spectralVectors);

            //FeaturePostProcessing.NormalizeMean(_vectors);
            //FeaturePostProcessing.AddDeltas(_vectors);

            var descriptions = tdExtractor.FeatureDescriptions
                                          .Concat(spectralExtractor.FeatureDescriptions);

            FillFeaturesList(_vectors, descriptions);
        }

        private void FillFeaturesList(IEnumerable<FeatureVector> featureVectors,
                                      IEnumerable<string> featureDescriptions)
        {
            featuresListView.Clear();
            featuresListView.Columns.Add("time", 50);

            foreach (var feat in featureDescriptions)
            {
                featuresListView.Columns.Add(feat, 70);
            }

            foreach (var vector in featureVectors)
            {
                var item = new ListViewItem { Text = vector.TimePosition.ToString("F4") };
                item.SubItems.AddRange(vector.Features.Select(f => f.ToString("F4")).ToArray());

                featuresListView.Items.Add(item);
            }
        }

        private void featuresListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
                return;
            }

            featurePlotPanel.Stride = 1;
            featurePlotPanel.Line = _vectors.Select(v => v.Features[e.Column - 1]).ToArray();
        }


        // TODO: remove this crap )))

        private void featuresListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fft = new Fft(512);

            var spectrum = fft.PowerSpectrum(_signal[3120, 3632]).Samples;

            var peaks = new int[10];
            var freqs = new float[10];

            Harmonic.Peaks(spectrum, peaks, freqs, _signal.SamplingRate);


            _spectrumImage = new Bitmap(512, spectrumPictureBox.Height);

            var g = Graphics.FromImage(_spectrumImage);
            g.Clear(Color.White);

            var pen = new Pen(ForeColor);
            var redpen = new Pen(Color.Red, 2);

            var i = 1;
            var Stride = 1;
            var PaddingX = 5;
            var PaddingY = 5;

            var x = PaddingX + Stride;

            var min = spectrum.Min();
            var max = spectrum.Max();

            var height = _spectrumImage.Height;
            var gain = max - min < 1e-6 ? 1 : (height - 2 * PaddingY) / (max - min);

            gain *= 100;

            var offset = (int)(height - PaddingY + min * gain);

            for (; i < spectrum.Length; i++)
            {
                g.DrawLine(pen, x - Stride, (float)(-spectrum[i - 1] * gain) + offset,
                                x, (float)(-spectrum[i] * gain) + offset);
                x += Stride;
            }

            for (i = 0; i < peaks.Length; i++)
            {
                g.DrawLine(redpen, PaddingX + peaks[i], (float)(-spectrum[peaks[i]] * gain) + offset,
                                   PaddingX + peaks[i], (float)(height - PaddingY) + offset);
            }

            pen.Dispose();
            redpen.Dispose();
            g.Dispose();

            spectrumPictureBox.Image = _spectrumImage;
        }

        Bitmap _spectrumImage;
    }
}
