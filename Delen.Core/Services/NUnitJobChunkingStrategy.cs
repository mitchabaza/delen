using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Delen.Core.Entities;
using NUnit.Core;

namespace Delen.Core.Services
{
    public class NUnitJobChunkingStrategy : IJobChunkingStrategy
    {
        private readonly int _degreeOfParallelism;
        private readonly string _tempDirectory;

        public NUnitJobChunkingStrategy(int degreeOfParallelism, string tempDirectory)
        {
            _degreeOfParallelism = degreeOfParallelism;
            _tempDirectory = tempDirectory;
        }

        public static List<T>[] Partition<T>(List<T> list, int totalPartitions)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (totalPartitions < 1)
                throw new ArgumentOutOfRangeException("totalPartitions");

            var partitions = new List<T>[totalPartitions];

            int maxSize = (int) Math.Ceiling(list.Count/(double) totalPartitions);
            int k = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                partitions[i] = new List<T>();
                for (int j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                        break;
                    partitions[i].Add(list[j]);
                }
                k += maxSize;
            }

            return partitions;
        }

        public IEnumerable<WorkItem> Chunk(Job job)
        {
            if (job.Arguments.Length == 0)
            {
                throw new InvalidOperationException("no assembly specified for Nunit");
            }
            var args = job.Arguments.Trim();
            // var assembly = args.Substring(0, args.IndexOf(" ", StringComparison.Ordinal));
            var assembly = args;
            var zipExtractedTo = WriteZip(job.WorkDirectoryArchive);

            var pathToTestAssembly = Path.Combine(zipExtractedTo, assembly);
            var nunit = new NUnitAssemblyInspector();
            var tests = nunit.ListAll(pathToTestAssembly);

            var enumerable = tests as string[] ?? tests.ToArray();
            if (enumerable.Count() <= _degreeOfParallelism)
            {
                return new[] {new WorkItem(job)};
            }

            var partitionedTests = Partition(enumerable.ToList(), _degreeOfParallelism);

            return
                partitionedTests.Select(
                    partitionedTest =>
                        new WorkItem(job, "/run=" + string.Join(",", partitionedTest.Select(x => x)) + " " + assembly))
                    .ToList();
        }

        private string WriteZip(byte[] bytes)
        {
            var tempFileName = _tempDirectory + @"\" + Path.GetRandomFileName();
            var tempFolder = _tempDirectory + @"\" + Path.GetRandomFileName();
            File.WriteAllBytes(tempFileName, bytes);
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
            ZipFile.ExtractToDirectory(tempFileName, tempFolder);
            return tempFolder;
        }
    }
}