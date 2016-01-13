﻿using Humanizer;
using System;
using System.Linq;

namespace Atata
{
    public abstract class QualifierFindAttribute : FindAttribute, IQualifierAttribute
    {
        protected QualifierFindAttribute(QualifierFormat format = QualifierFormat.Inherit)
        {
            Format = format;
        }

        protected QualifierFindAttribute(params string[] values)
        {
            Values = values;
        }

        public string[] Values { get; private set; }
        public QualifierFormat Format { get; private set; }
        public bool ApplyNameAsIs { get; set; }

        public virtual string[] GetQualifiers(UIPropertyMetadata metadata)
        {
            StringValueAttribute stringValueAttribute;

            if (Values != null && Values.Any())
                return Values;
            else if (metadata.Property.TryGetCustomAttribute(out stringValueAttribute) && stringValueAttribute.Values != null && stringValueAttribute.Values.Any())
                return stringValueAttribute.Values;
            else
                return new[] { GetQualifierFromProperty(metadata) };
        }

        private string GetQualifierFromProperty(UIPropertyMetadata metadata)
        {
            QualifierFormat format = GetQualifierFormat(metadata);
            string name = GetPropertyName(metadata);
            return Humanize(name, format);
        }

        private QualifierFormat GetQualifierFormat(UIPropertyMetadata metadata)
        {
            return Format != QualifierFormat.Inherit ? Format : GetQualifierFormatFromMetadata(metadata);
        }

        private string GetPropertyName(UIPropertyMetadata metadata)
        {
            string name = metadata.Property.Name;
            if (!ApplyNameAsIs)
            {
                string suffixToIgnore = metadata.ComponentAttribute.GetIgnoreNameEndingValues().
                    FirstOrDefault(x => name.EndsWith(x) && name.Length > x.Length);

                if (suffixToIgnore != null)
                    return name.Substring(0, name.Length - suffixToIgnore.Length);
            }
            return name;
        }

        protected abstract QualifierFormat GetQualifierFormatFromMetadata(UIPropertyMetadata metadata);

        private static string Humanize(string name, QualifierFormat format)
        {
            switch (format)
            {
                case QualifierFormat.Title:
                    return name.Humanize(LetterCasing.Title);
                case QualifierFormat.Sentence:
                    return name.Humanize(LetterCasing.Sentence);
                case QualifierFormat.LowerCase:
                    return name.Humanize(LetterCasing.LowerCase);
                case QualifierFormat.UpperCase:
                    return name.Humanize(LetterCasing.AllCaps);
                case QualifierFormat.Camel:
                    return name.Humanize().Camelize();
                case QualifierFormat.Pascal:
                    return name.Humanize().Pascalize();
                case QualifierFormat.Dashed:
                    return name.Humanize().Dasherize();
                case QualifierFormat.XDashed:
                    return "x-" + name.Humanize().Dasherize();
                case QualifierFormat.Underscored:
                    return name.Humanize().Underscore();
                default:
                    throw new ArgumentException("Unknown format", "format");
            }
        }
    }
}