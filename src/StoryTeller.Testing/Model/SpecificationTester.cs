﻿using System;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using NUnit.Framework;
using Shouldly;
using StoryTeller.Model;
using StoryTeller.Model.Persistence;
using StoryTeller.Remotes.Messaging;

namespace StoryTeller.Testing.Model
{
    [TestFixture]
    public class SpecificationTester
    {
        [Test]
        public void is_full_by_default()
        {
            new Specification().SpecType
                .ShouldBe(SpecType.full);
        }

        [Test]
        public void read_spec_data_from_header_mode()
        {
            var path = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                .AppendPath("Storyteller.Samples", "Specs", "General", "Check properties.xml");

            var spec = HierarchyLoader.ReadSpecHeader(path);
            spec.Children.Any().ShouldBeFalse();

            spec.SpecType.ShouldBe(SpecType.header);

            spec.ReadBody();

            spec.SpecType.ShouldBe(SpecType.full);

            spec.Children.Any().ShouldBeTrue();


        }

        [Test]
        public void serializes_fine()
        {
            var spec = new Specification();
            var c1 = new Comment();
            spec.Children.Add(c1);
            var section1 = new Section("Foo");
            var s1 = section1.AddStep("foo1");
            var s2 = section1.AddStep("foo1");
            var s3 = section1.AddStep("foo1");


            var section2 = s3.AddCollection("rows");
            var s4 = section2.AddStep("r1");
            var s5 = section2.AddStep("r1");
            var s6 = section2.AddStep("r1");

            spec.Children.Add(section1);
            var c2 = new Comment();
            spec.Children.Add(c2);

            var json = JsonSerialization.ToJson(spec, true);

            Debug.WriteLine(json);
        }

        [Test]
        public void can_find_all_children()
        {
            var spec = new Specification();
            var c1 = new Comment();
            spec.Children.Add(c1);
            var section1 = new Section("Foo");
            var s1 = section1.AddStep("foo1");
            var s2 = section1.AddStep("foo1");
            var s3 = section1.AddStep("foo1");


            var section2 = s3.AddCollection("rows");
            var s4 = section2.AddStep("r1");
            var s5 = section2.AddStep("r1");
            var s6 = section2.AddStep("r1");

            spec.Children.Add(section1);
            var c2 = new Comment();
            spec.Children.Add(c2);

            var nodes = spec.AllNodes();

            // All comments, steps, sections, and the spec itself
            nodes.Count().ShouldBe(11);

            nodes.ShouldContain(spec);
            nodes.ShouldContain(section1);
            nodes.ShouldContain(section2);
            nodes.ShouldContain(c1);
            nodes.ShouldContain(c2);
            nodes.ShouldContain(s1);
            nodes.ShouldContain(s2);
            nodes.ShouldContain(s3);
            nodes.ShouldContain(s4);
            nodes.ShouldContain(s5);
            nodes.ShouldContain(s6);
        }

        [Test]
        public void needs_renumbering_negative()
        {
            var spec = new Specification();
            var c1 = new Comment();
            spec.Children.Add(c1);
            var section1 = new Section("Foo");
            var s1 = section1.AddStep("foo1");
            var s2 = section1.AddStep("foo1");
            var s3 = section1.AddStep("foo1");


            var section2 = s3.AddCollection("rows");
            var s4 = section2.AddStep("r1");
            var s5 = section2.AddStep("r1");
            var s6 = section2.AddStep("r1");

            spec.Children.Add(section1);
            var c2 = new Comment();
            spec.Children.Add(c2);

            spec.NeedsToBeRenumbered().ShouldBeFalse();
        }


        [Test]
        public void needs_renumbering_is_true_because_there_are_duplicate_ids()
        {
            var spec = new Specification();
            var c1 = new Comment();
            spec.Children.Add(c1);
            var section1 = new Section("Foo");
            var s1 = section1.AddStep("foo1");
            var s2 = section1.AddStep("foo1");
            var s3 = section1.AddStep("foo1");


            var section2 = s3.AddCollection("rows");
            var s4 = section2.AddStep("r1");
            var s5 = section2.AddStep("r1");
            var s6 = section2.AddStep("r1");

            s4.id = s5.id = s6.id = Guid.NewGuid().ToString();

            spec.Children.Add(section1);
            var c2 = new Comment();
            spec.Children.Add(c2);

            spec.NeedsToBeRenumbered().ShouldBeTrue();
        }

        [Test]
        public void apply_renumbering()
        {
            var spec = new Specification();
            var c1 = new Comment();
            spec.Children.Add(c1);
            var section1 = new Section("Foo");
            var s1 = section1.AddStep("foo1");
            var s2 = section1.AddStep("foo1");
            var s3 = section1.AddStep("foo1");


            var section2 = s3.AddCollection("rows");
            var s4 = section2.AddStep("r1");
            var s5 = section2.AddStep("r1");
            var s6 = section2.AddStep("r1");

            s4.id = s5.id = s6.id = Guid.NewGuid().ToString();

            spec.Children.Add(section1);
            var c2 = new Comment();
            spec.Children.Add(c2);

            spec.ApplyRenumbering();

            s4.id.ShouldNotBe(s5.id);
            s5.id.ShouldNotBe(s6.id);
            s4.id.ShouldNotBe(s6.id);
        }
    }
}