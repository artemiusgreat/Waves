﻿using System;
using System.Linq;
using NWaves.Signals;

namespace NWaves.Filters.Adaptive
{
    ///// <summary>
    ///// Adaptive filter (Recursive Least-Squares algorithm)
    ///// </summary>
    //private class RlsFilter
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    private readonly int _order;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    private readonly float _mu;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    public RlsFilter(int order, float mu)
    //    {
    //        _order = order;
    //        _mu = mu;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="signal"></param>
    //    /// <param name="desired"></param>
    //    /// <returns></returns>
    //    public DiscreteSignal Adapt(DiscreteSignal signal, DiscreteSignal desired)
    //    {
    //        var rand = new Random();
    //        var kernel = Enumerable.Range(0, _order)
    //                               .Select(k => (float)(1.0 + rand.NextDouble()))
    //                               .ToArray();

    //        var input = signal.Samples;
    //        var output = new float[input.Length];

    //        var pos = _order;

    //        for (var i = 0; i + _order < input.Length; i++)
    //        {
    //            var y = 0.0f;
    //            for (var j = 0; j < _order; j++)
    //            {
    //                y += input[i + j] * kernel[_order - j];
    //            }

    //            output[pos] = y;

    //            var error = desired[i] - y;

    //            var k = 1.0f;

    //            for (var j = 0; j < _order; j++)
    //            {
    //                kernel[j] += k * error;
    //            }
    //        }

    //        return new DiscreteSignal(signal.SamplingRate, output);
    //    }
    //}
}
