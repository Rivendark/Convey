using System;
using System.Text;

namespace Convey.Persistence.InfluxDB
{
    public sealed class FluxQueryBuilder
    {
        private StringBuilder _queryStringBuilder = new StringBuilder();
        private StringBuilder _filterStringBuilder = new StringBuilder();
        private StringBuilder _rangeStringBuilder = new StringBuilder();
        private StringBuilder _sampleStringBuilder = new StringBuilder();
        private bool _rangeSet;
        private bool _sampleSet;
        public string BucketName { get; }

        internal FluxQueryBuilder(string bucketName)
        {
            BucketName = bucketName;
        }

        internal string GetQuery()
        {
            _queryStringBuilder.Append($"from(bucket: \"{BucketName}\")\n");
            if (_filterStringBuilder.Length > 0)
                _queryStringBuilder.Append($" |> filter(fn: (r) => ({_filterStringBuilder})");
            if (_rangeStringBuilder.Length > 0)
                _queryStringBuilder.Append($" |> {_rangeStringBuilder}");
            if (_sampleStringBuilder.Length > 0)
                _queryStringBuilder.Append($" |> {_sampleStringBuilder}");

            return _queryStringBuilder.ToString();
        }

        public FluxQueryBuilder AddSample(int length, int? offset = null)
        {
            if (_sampleSet)
            {
                throw new ArgumentException($"Sample already set. {_sampleStringBuilder}");
            }
            _sampleStringBuilder.Append($"sample(n: {length}");
            if (offset.HasValue)
            {
                _sampleStringBuilder.Append($", n: {offset}");
            }

            _sampleStringBuilder.Append(")");
            _sampleSet = true;
            
            return this;
        }
        
        public FluxQueryBuilder AddRangeStartOnly(string start)
        {
            if (_rangeSet)
            {
                throw new ArgumentException($"Range already set. {_rangeStringBuilder}");
            }
            _rangeStringBuilder.Append($"range(start: {start})");
            _rangeSet = true;
            
            return this;
        }
        
        public FluxQueryBuilder AddRangeEndOnly(string end)
        {
            if (_rangeSet)
            {
                throw new ArgumentException($"Range already set. {_rangeStringBuilder}");
            }
            _rangeStringBuilder.Append($"range(end: {end})");
            _rangeSet = true;
            
            return this;
        }
        
        public FluxQueryBuilder AddRange(string start, string end)
        {
            if (string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end))
            {
                throw new Exception("Start and End must have values.");
            }
            
            if (_rangeSet)
            {
                throw new ArgumentException($"Range already set. {_rangeStringBuilder}");
            }
            _rangeStringBuilder.Append($"range(start: {start}, end:{end})");
            _rangeSet = true;
            
            return this;
        }

        public FluxQueryBuilder AddFilter(string fieldName, FluxEquationOperators @operator, string value)
        {
            _filterStringBuilder.Append($"r[\"{fieldName}\"] ");
            _filterStringBuilder.Append(GetOperatorString(@operator));
            _filterStringBuilder.Append($"\"{value}\"");
            
            return this;
        }

        public FluxQueryBuilder AddFilterAnd()
        {
            _filterStringBuilder.Append(" AND ");
            
            return this;
        }

        public FluxQueryBuilder AddFilterOr()
        {
            _filterStringBuilder.Append(" OR ");
            
            return this;
        }

        public FluxQueryBuilder AddFilterStartParens()
        {
            _filterStringBuilder.Append("(");
            
            return this;
        }

        public FluxQueryBuilder AddFilterEndParens()
        {
            _filterStringBuilder.Append(")");

            return this;
        }

        private static string GetOperatorString(FluxEquationOperators @operator)
        {
            string op;
            switch (@operator)
            {
                case FluxEquationOperators.Equals:
                    op = "== ";
                    break;
                case FluxEquationOperators.NotEquals:
                    op = "!= ";
                    break;
                case FluxEquationOperators.GreaterThen:
                    op = "> ";
                    break;
                case FluxEquationOperators.GreaterThenOrEquals:
                    op = ">= ";
                    break;
                case FluxEquationOperators.LessThen:
                    op = "< ";
                    break;
                case FluxEquationOperators.LessThenOrEquals:
                    op = "<= ";
                    break;
                case FluxEquationOperators.In:
                    op = "IN";
                    break;
                case FluxEquationOperators.NotIn:
                    op = "NotIn";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
            }

            return op;
        }
    }
}