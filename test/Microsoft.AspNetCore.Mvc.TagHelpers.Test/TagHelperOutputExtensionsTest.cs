// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.TagHelpers
{
    public class TagHelperOutputExtensionsTest
    {
        public static TheoryData CopyHtmlAttributeData_MaintainsOrder
        {
            get
            {
                // attributeNameToCopy, outputAttributes, allAttributes, expectedAttributes
                return new TheoryData<
                    string,
                    TagHelperAttributeList,
                    TagHelperAttributeList,
                    IEnumerable<TagHelperAttribute>>
                {
                    {
                        "first",
                        new TagHelperAttributeList
                        {
                            { "second", "B" },
                        },
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "second", "B" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("second", "B"),
                        }
                    },
                    {
                        "second",
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                        },
                        new TagHelperAttributeList
                        {
                            { "second", "B" },
                            { "second", "Duplicate B" },
                            { "first", "A" },
                        },
                        new[]
                        {
                            new TagHelperAttribute("second", "B"),
                            new TagHelperAttribute("second", "Duplicate B"),
                            new TagHelperAttribute("first", "A"),
                        }
                    },
                    {
                        "second",
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                        },
                        new TagHelperAttributeList
                        {
                            { "second", "B" },
                            { "first", "A" },
                            { "second", "Duplicate B" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("second", "B"),
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("second", "Duplicate B"),
                        }
                    },
                    {
                        "dynamic",
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "second", "B" },
                        },
                        new TagHelperAttributeList
                        {
                            { "dynamic", "value" },
                        },
                        new[]
                        {
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("second", "B"),
                            new TagHelperAttribute("dynamic", "value"),
                        }
                    },
                    {
                        "second",
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "dynamic", "value" },
                            { "secondDynamic", "another value"}
                        },
                        new TagHelperAttributeList
                        {
                            { "second", "B" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("dynamic", "value"),
                            new TagHelperAttribute("secondDynamic", "another value"),
                            new TagHelperAttribute("second", "B"),
                        }
                    },
                    {
                        "second",
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "dynamic", "value" },
                            { "secondDynamic", "another value"}
                        },
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "second", "B" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("second", "B"),
                            new TagHelperAttribute("dynamic", "value"),
                            new TagHelperAttribute("secondDynamic", "another value"),
                        }
                    },
                    {
                        "third",
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "dynamic", "value" },
                            { "secondDynamic", "another value"}
                        },
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "second", "B" },
                            { "third", "C" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("third", "C"),
                            new TagHelperAttribute("dynamic", "value"),
                            new TagHelperAttribute("secondDynamic", "another value"),
                        }
                    },
                    {
                        "first",
                        new TagHelperAttributeList
                        {
                            { "third", "C" },
                            { "dynamic", "value" },
                            { "secondDynamic", "another value"}
                        },
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "second", "B" },
                            { "third", "C" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("third", "C"),
                            new TagHelperAttribute("dynamic", "value"),
                            new TagHelperAttribute("secondDynamic", "another value"),
                        }
                    },
                    {
                        "first",
                        new TagHelperAttributeList
                        {
                            { "third", "C" },
                            { "dynamic", "value" },
                            { "secondDynamic", "another value"}
                        },
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "secondDynamic", "another value"},
                            { "second", "B" },
                            { "third", "C" },
                        },
                        new[]
                        {
                            new TagHelperAttribute("third", "C"),
                            new TagHelperAttribute("dynamic", "value"),
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("secondDynamic", "another value"),
                        }
                    },
                    {
                        "first",
                        new TagHelperAttributeList
                        {
                            { "third", "C" },
                            { "dynamic", "value" },
                            { "secondDynamic", "another value"}
                        },
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "secondDynamic", "another value"},
                            { "first", "Second first" },
                            { "second", "B" },
                            { "third", "C" },
                            { "first", "third first" },
                        },
                        new[]
                        {
                            new TagHelperAttribute("third", "C"),
                            new TagHelperAttribute("first", "third first"),
                            new TagHelperAttribute("dynamic", "value"),
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("secondDynamic", "another value"),
                            new TagHelperAttribute("first", "Second first"),
                        }
                    },
                    {
                        "first",
                        new TagHelperAttributeList
                        {
                            { "third", "C" },
                            { "third", "Duplicate Third" },
                        },
                        new TagHelperAttributeList
                        {
                            { "third", "C" },
                            { "first", "A" },
                            { "third", "Duplicate Third" },
                        },
                        new[]
                        {
                            new TagHelperAttribute("third", "C"),
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("third", "Duplicate Third"),
                        }
                    },
                    {
                        "first",
                        new TagHelperAttributeList
                        {
                            { "third", "C" },
                            { "third", "Duplicate Third" },
                        },
                        new TagHelperAttributeList
                        {
                            { "third", "C" },
                            { "third", "Duplicate Third" },
                            { "first", "A" },
                        },
                        new[]
                        {
                            new TagHelperAttribute("third", "C"),
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("third", "Duplicate Third"),
                        }
                    },
                    {
                        "first",
                        new TagHelperAttributeList
                        {
                            { "third", "D" },
                        },
                        new TagHelperAttributeList
                        {
                            { "first", "A" },
                            { "first", "B" },
                            { "first", "C" },
                            { "third", "D" },
                        },
                        new[]
                        {
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("first", "C"),
                            new TagHelperAttribute("first", "B"),
                            new TagHelperAttribute("third", "D"),
                        }
                    },
                    {
                        "first",
                        new TagHelperAttributeList
                        {
                            { "third", "D" },
                            { "dynamic", "value" },
                            { "third", "Duplicate Third" },
                        },
                        new TagHelperAttributeList
                        {
                            { "third", "D" },
                            { "first", "A" },
                            { "third", "Duplicate Third" },
                            { "first", "B" },
                            { "first", "C" },
                        },
                        new[]
                        {
                            new TagHelperAttribute("third", "D"),
                            new TagHelperAttribute("first", "A"),
                            new TagHelperAttribute("first", "B"),
                            new TagHelperAttribute("first", "C"),
                            new TagHelperAttribute("dynamic", "value"),
                            new TagHelperAttribute("third", "Duplicate Third"),
                        }
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(CopyHtmlAttributeData_MaintainsOrder))]
        public void CopyHtmlAttribute_MaintainsOrder(
            string attributeNameToCopy,
            TagHelperAttributeList outputAttributes,
            TagHelperAttributeList allAttributes,
            IEnumerable<TagHelperAttribute> expectedAttributes)
        {
            // Arrange
            var output = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(outputAttributes),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            var context = new TagHelperContext(
                allAttributes,
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            // Act
            output.CopyHtmlAttribute(attributeNameToCopy, context);

            // Assert
            Assert.Equal(expectedAttributes, output.Attributes, CaseSensitiveTagHelperAttributeComparer.Default);
        }

        public static TheoryData CopyHtmlAttributeData_MultipleAttributesSameName
        {
            get
            {
                // attributeNameToCopy, allAttributes, expectedAttributes
                return new TheoryData<string, TagHelperAttributeList, IEnumerable<TagHelperAttribute>>
                {
                    {
                        "hello",
                        new TagHelperAttributeList
                        {
                            { "hello", "world" },
                            { "hello", "world2" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("hello", "world"),
                            new TagHelperAttribute("hello", "world2"),
                        }
                    },
                    {
                        "HELLO",
                        new TagHelperAttributeList
                        {
                            { "hello", "world" },
                            { "hello", "world2" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("hello", "world"),
                            new TagHelperAttribute("hello", "world2"),
                        }
                    },
                    {
                        "hello",
                        new TagHelperAttributeList
                        {
                            { "HelLO", "world" },
                            { "HELLO", "world2" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("HelLO", "world"),
                            new TagHelperAttribute("HELLO", "world2"),
                        }
                    },
                    {
                        "hello",
                        new TagHelperAttributeList
                        {
                            { "hello", "world" },
                            { "HELLO", "world2" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("hello", "world"),
                            new TagHelperAttribute("HELLO", "world2"),
                        }
                    },
                    {
                        "HELLO",
                        new TagHelperAttributeList
                        {
                            { "HeLlO", "world" },
                            { "heLLo", "world2" }
                        },
                        new[]
                        {
                            new TagHelperAttribute("HeLlO", "world"),
                            new TagHelperAttribute("heLLo", "world2"),
                        }
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(CopyHtmlAttributeData_MultipleAttributesSameName))]
        public void CopyHtmlAttribute_CopiesAllOriginalAttributes(
            string attributeNameToCopy,
            TagHelperAttributeList allAttributes,
            IEnumerable<TagHelperAttribute> expectedAttributes)
        {
            // Arrange
            var output = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            var context = new TagHelperContext(
                allAttributes,
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            // Act
            output.CopyHtmlAttribute(attributeNameToCopy, context);

            // Assert
            Assert.Equal(expectedAttributes, output.Attributes, CaseSensitiveTagHelperAttributeComparer.Default);
        }

        [Theory]
        [InlineData("hello", "world")]
        [InlineData("HeLlO", "wOrLd")]
        public void CopyHtmlAttribute_CopiesOriginalAttributes(string attributeName, string attributeValue)
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.Append("Something");
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });
            var tagHelperContext = new TagHelperContext(
                allAttributes: new TagHelperAttributeList
                {
                    { attributeName, attributeValue }
                },
                items: new Dictionary<object, object>(),
                uniqueId: "test");
            var expectedAttribute = new TagHelperAttribute(attributeName, attributeValue);

            // Act
            tagHelperOutput.CopyHtmlAttribute("hello", tagHelperContext);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute);
        }

        [Fact]
        public void CopyHtmlAttribute_DoesNotOverrideAttributes()
        {
            // Arrange
            var attributeName = "hello";
            var tagHelperOutput = new TagHelperOutput(
                "p",
                attributes: new TagHelperAttributeList()
                {
                    { attributeName, "world2" }
                },
                getChildContentAsync: (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.Append("Something Else");
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });
            var expectedAttribute = new TagHelperAttribute(attributeName, "world2");
            var tagHelperContext = new TagHelperContext(
                allAttributes: new TagHelperAttributeList
                {
                    { attributeName, "world" }
                },
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            // Act
            tagHelperOutput.CopyHtmlAttribute(attributeName, tagHelperContext);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute);
        }

        [Fact]
        public void CopyHtmlAttribute_ThrowsWhenUnknownAttribute()
        {
            // Arrange
            var invalidAttributeName = "hello2";
            var tagHelperOutput = new TagHelperOutput(
                "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.Append("Something");
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });
            var tagHelperContext = new TagHelperContext(
                allAttributes: new TagHelperAttributeList
                {
                    { "hello", "world" }
                },
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            // Act & Assert
            ExceptionAssert.ThrowsArgument(
                () => tagHelperOutput.CopyHtmlAttribute(invalidAttributeName, tagHelperContext),
                "attributeName",
                "The attribute 'hello2' does not exist in the TagHelperContext.");
        }

        [Fact]
        public void RemoveRange_RemovesProvidedAttributes_WithArrayInput()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList()
                {
                    { "route-Hello", "World" },
                    { "Route-I", "Am" }
                },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            var expectedAttribute = new TagHelperAttribute("type", "btn");
            tagHelperOutput.Attributes.Add(expectedAttribute);

            var attributes = tagHelperOutput.Attributes
                .Where(item => item.Name.StartsWith("route-", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Act
            tagHelperOutput.RemoveRange(attributes);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute);
        }

        [Fact]
        public void RemoveRange_RemovesProvidedAttributes_WithCollectionInput()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList()
                {
                    { "route-Hello", "World" },
                    { "Route-I", "Am" }
                },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            var expectedAttribute = new TagHelperAttribute("type", "btn");
            tagHelperOutput.Attributes.Add(expectedAttribute);
            var attributes = tagHelperOutput.Attributes
                .Where(item => item.Name.StartsWith("route-", StringComparison.OrdinalIgnoreCase));

            // Act
            tagHelperOutput.RemoveRange(attributes);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute);
        }

        [Fact]
        public void FindPrefixedAttributes_ReturnsEmpty_AttributeListIfNoAttributesPrefixed()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList()
                {
                    { "route-Hello", "World" },
                    { "Route-I", "Am" }
                },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            var expectedAttribute = new TagHelperAttribute("type", "btn");
            tagHelperOutput.Attributes.Add(expectedAttribute);

            var attributes = tagHelperOutput.Attributes
                .Where(item => item.Name.StartsWith("route-", StringComparison.OrdinalIgnoreCase));

            // Act
            tagHelperOutput.RemoveRange(attributes);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute);
        }

        public static TheoryData MultipleAttributeSameNameData
        {
            get
            {
                // tagBuilderAttributes, outputAttributes, expectedAttributes
                return new TheoryData<Dictionary<string, string>, TagHelperAttributeList, TagHelperAttributeList>
                {
                    {
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "class", "btn" }
                        },
                        new TagHelperAttributeList
                        {
                            { "class", "btn2" },
                            { "class", "btn3" }
                        },
                        new TagHelperAttributeList
                        {
                            { "class", "btn2 btn" }
                        }
                    },
                    {
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "ClAsS", "btn" }
                        },
                        new TagHelperAttributeList
                        {
                            { "class", "btn2" },
                            { "class", "btn3" }
                        },
                        new TagHelperAttributeList
                        {
                            { "class", "btn2 btn" }
                        }
                    },
                    {
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "class", "btn" }
                        },
                        new TagHelperAttributeList
                        {
                            { "clASS", "btn2" },
                            { "class", "btn3" }
                        },
                        new TagHelperAttributeList
                        {
                            { "clASS", "btn2 btn" }
                        }
                    },
                    {
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "class", "btn" }
                        },
                        new TagHelperAttributeList
                        {
                            { "clASS", "btn2" },
                            { "CLass", "btn3" }
                        },
                        new TagHelperAttributeList
                        {
                            { "clASS", "btn2 btn" }
                        }
                    },
                    {
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "CLASS", "btn" }
                        },
                        new TagHelperAttributeList
                        {
                            { "clASS", "btn2" },
                            { "CLass", "btn3" }
                        },
                        new TagHelperAttributeList
                        {
                            { "clASS", "btn2 btn" }
                        }
                    },
                    {
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "CLASS", "btn" }
                        },
                        new TagHelperAttributeList
                        {
                            { "before", "before value" },
                            { "clASS", "btn2" },
                            { "mid", "mid value" },
                            { "CLass", "btn3" },
                            { "after", "after value" },
                        },
                        new TagHelperAttributeList
                        {
                            { "before", "before value" },
                            { "clASS", "btn2 btn" },
                            { "mid", "mid value" },
                            { "after", "after value" },
                        }
                    },
                    {
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "A", "A Value" },
                            { "CLASS", "btn" },
                            { "B", "B Value" },
                        },
                        new TagHelperAttributeList
                        {
                            { "before", "before value" },
                            { "clASS", "btn2" },
                            { "mid", "mid value" },
                            { "CLass", "btn3" },
                            { "after", "after value" },
                        },
                        new TagHelperAttributeList
                        {
                            { "before", "before value" },
                            { "clASS", "btn2 btn" },
                            { "mid", "mid value" },
                            { "after", "after value" },
                            { "A", "A Value" },
                            { "B", "B Value" },
                        }
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(MultipleAttributeSameNameData))]
        public void MergeAttributes_ClearsDuplicateClassNameAttributes(
            Dictionary<string, string> tagBuilderAttributes,
            TagHelperAttributeList outputAttributes,
            TagHelperAttributeList expectedAttributes)
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                "p",
                outputAttributes,
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));

            var tagBuilder = new TagBuilder("p");
            foreach (var attr in tagBuilderAttributes)
            {
                tagBuilder.Attributes.Add(attr.Key, attr.Value);
            }

            // Act
            tagHelperOutput.MergeAttributes(tagBuilder);

            // Assert
            Assert.Equal(expectedAttributes, tagHelperOutput.Attributes, TagHelperAttributeStringComparer.Instance);
        }

        [Fact]
        public void MergeAttributes_DoesNotReplace_TagHelperOutputAttributeValues()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            var expectedAttribute = new TagHelperAttribute("type", "btn");
            tagHelperOutput.Attributes.Add(expectedAttribute);

            var tagBuilder = new TagBuilder("p");
            tagBuilder.Attributes.Add("type", "hello");

            // Act
            tagHelperOutput.MergeAttributes(tagBuilder);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute);
        }

        [Fact]
        public void MergeAttributes_AppendsClass_TagHelperOutputAttributeValues()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            tagHelperOutput.Attributes.Add("class", "Hello");

            var tagBuilder = new TagBuilder("p");
            tagBuilder.Attributes.Add("class", "btn");

            var expectedAttribute = new TagHelperAttribute("class", "Hello btn");

            // Act
            tagHelperOutput.MergeAttributes(tagBuilder);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute, TagHelperAttributeStringComparer.Instance);
        }

        [Theory]
        [InlineData("class", "CLAss")]
        [InlineData("ClaSS", "class")]
        [InlineData("ClaSS", "cLaSs")]
        public void MergeAttributes_AppendsClass_TagHelperOutputAttributeValues_IgnoresCase(
            string originalName, string updateName)
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            tagHelperOutput.Attributes.Add(originalName, "Hello");

            var tagBuilder = new TagBuilder("p");
            tagBuilder.Attributes.Add(updateName, "btn");

            // Act
            tagHelperOutput.MergeAttributes(tagBuilder);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(
                new TagHelperAttribute(originalName, "Hello btn"),
                attribute,
                TagHelperAttributeStringComparer.Instance);
        }

        [Fact]
        public void MergeAttributes_DoesNotEncode_TagHelperOutputAttributeValues()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));

            var tagBuilder = new TagBuilder("p");
            var expectedAttribute = new TagHelperAttribute("visible", "val < 3");
            tagBuilder.Attributes.Add("visible", "val < 3");

            // Act
            tagHelperOutput.MergeAttributes(tagBuilder);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute, TagHelperAttributeStringComparer.Instance);
        }

        [Fact]
        public void MergeAttributes_CopiesMultiple_TagHelperOutputAttributeValues()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));

            var tagBuilder = new TagBuilder("p");
            var expectedAttribute1 = new TagHelperAttribute("class", "btn");
            var expectedAttribute2 = new TagHelperAttribute("class2", "btn");
            tagBuilder.Attributes.Add("class", "btn");
            tagBuilder.Attributes.Add("class2", "btn");

            // Act
            tagHelperOutput.MergeAttributes(tagBuilder);

            // Assert
            Assert.Equal(2, tagHelperOutput.Attributes.Count);
            var attribute = Assert.Single(tagHelperOutput.Attributes, attr => attr.Name.Equals("class"));
            Assert.Equal(expectedAttribute1, attribute, TagHelperAttributeStringComparer.Instance);
            attribute = Assert.Single(tagHelperOutput.Attributes, attr => attr.Name.Equals("class2"));
            Assert.Equal(expectedAttribute2, attribute, TagHelperAttributeStringComparer.Instance);
        }

        [Fact]
        public void MergeAttributes_Maintains_TagHelperOutputAttributeValues()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            var expectedAttribute = new TagHelperAttribute("class", "btn");
            tagHelperOutput.Attributes.Add(expectedAttribute);

            var tagBuilder = new TagBuilder("p");

            // Act
            tagHelperOutput.MergeAttributes(tagBuilder);

            // Assert
            var attribute = Assert.Single(tagHelperOutput.Attributes);
            Assert.Equal(expectedAttribute, attribute);
        }

        [Fact]
        public void MergeAttributes_Combines_TagHelperOutputAttributeValues()
        {
            // Arrange
            var tagHelperOutput = new TagHelperOutput(
                tagName: "p",
                attributes: new TagHelperAttributeList(),
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            var expectedOutputAttribute = new TagHelperAttribute("class", "btn");
            tagHelperOutput.Attributes.Add(expectedOutputAttribute);

            var tagBuilder = new TagBuilder("p");
            var expectedBuilderAttribute = new TagHelperAttribute("for", "hello");
            tagBuilder.Attributes.Add("for", "hello");

            // Act
            tagHelperOutput.MergeAttributes(tagBuilder);

            // Assert
            Assert.Equal(tagHelperOutput.Attributes.Count, 2);
            var attribute = Assert.Single(tagHelperOutput.Attributes, attr => attr.Name.Equals("class"));
            Assert.Equal(expectedOutputAttribute, attribute, TagHelperAttributeStringComparer.Instance);
            attribute = Assert.Single(tagHelperOutput.Attributes, attr => attr.Name.Equals("for"));
            Assert.Equal(expectedBuilderAttribute, attribute, TagHelperAttributeStringComparer.Instance);
        }

        private class CaseSensitiveTagHelperAttributeComparer : IEqualityComparer<TagHelperAttribute>
        {
            public readonly static CaseSensitiveTagHelperAttributeComparer Default =
                new CaseSensitiveTagHelperAttributeComparer();

            private CaseSensitiveTagHelperAttributeComparer()
            {
            }

            public bool Equals(TagHelperAttribute attributeX, TagHelperAttribute attributeY)
            {
                if (attributeX == attributeY)
                {
                    return true;
                }

                // Normal comparer (TagHelperAttribute.Equals()) doesn't care about the Name case, in tests we do.
                return attributeX != null &&
                    string.Equals(attributeX.Name, attributeY.Name, StringComparison.Ordinal) &&
                    attributeX.ValueStyle == attributeY.ValueStyle &&
                    (attributeX.ValueStyle == HtmlAttributeValueStyle.Minimized || Equals(attributeX.Value, attributeY.Value));
            }

            public int GetHashCode(TagHelperAttribute attribute)
            {
                // Manually combine hash codes here. We can't reference HashCodeCombiner because we have internals visible
                // from Mvc.Core and Mvc.TagHelpers; both of which reference HashCodeCombiner.
                var baseHashCode = 0x1505L;
                var attributeHashCode = attribute.GetHashCode();
                var combinedHash = ((baseHashCode << 5) + baseHashCode) ^ attributeHashCode;
                var nameHashCode = StringComparer.Ordinal.GetHashCode(attribute.Name);
                combinedHash = ((combinedHash << 5) + combinedHash) ^ nameHashCode;

                return combinedHash.GetHashCode();
            }
        }
    }
}
