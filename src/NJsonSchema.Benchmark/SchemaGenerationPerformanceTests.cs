﻿using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace NJsonSchema.Benchmark
{
    public class SchemaGenerationPerformanceTests
    {
        private Counter _counter;

        public SchemaGenerationPerformanceTests(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
#pragma warning disable xUnit1013 // Public method should be marked as test
        public void Setup(BenchmarkContext context)
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            _counter = context.GetCounter("Iterations");
        }

        /// <summary>
        /// Ensure that we can serialise at least 200 times per second (5ms).
        /// </summary>
        [NBenchFact]
        [PerfBenchmark(
            Description = "Ensure schema generation doesn't take too long",
            NumberOfIterations = 3,
            RunTimeMilliseconds = 1000,
            RunMode = RunMode.Throughput,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion("Iterations", MustBe.GreaterThan, 100)]
        public void GenerateSchema()
        {
            var schema = JsonSchema.FromType<Container>();
            _counter.Increment();
        }

        public class SpecialTeacher : Teacher
        {
            public string Foo { get; set; }
        }

        [KnownType(typeof(SpecialTeacher))]
        public class Teacher
        {
            public string Bar { get; set; }
        }

        [KnownType(typeof(Teacher))]
        public class Person
        {
            public string Baz { get; set; }
        }

        public class Pen : WritingInstrument
        {
            public string Foo { get; set; }
        }

        public class Pencil : WritingInstrument
        {
            public string Bar { get; set; }
        }

        [KnownType("GetKnownTypes")]
        public class WritingInstrument
        {
            public static Type[] GetKnownTypes()
            {
                return new[] { typeof(Pen), typeof(Pencil) };
            }

            public string Baz { get; set; }
        }

        public class Container
        {
            public Person Person { get; set; }

            public Teacher Teacher { get; set; }

            public WritingInstrument WritingInstrument { get; set; }
        }
    }
}
