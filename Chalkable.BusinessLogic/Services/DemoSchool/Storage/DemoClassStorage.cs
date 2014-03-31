using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassStorage:BaseDemoStorage<int , Class>
    {
        public DemoClassStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(Class cClass)
        {
            if (!data.ContainsKey(cClass.Id))
                data[cClass.Id] = cClass;
        }

        public void Add(IList<Class> classes)
        {
            foreach (var cls in classes)
            {
                Add(cls);
            }
        }

        public void Update(Class cClass)
        {
            if (data.ContainsKey(cClass.Id))
                data[cClass.Id] = cClass;
        }

        public void Update(IList<Class> classes)
        {
            foreach (var cls in classes)
            {
                Update(cls);
            }
        }

        public void Setup()
        {
            var classes = new List<Class>();
            classes.Add(new Class
            {
                Id = 1,
                Name = "Elementary",
                Description = "Elementary Classrooms",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 4,
                Name = "Math 6",
                Description = "6th Grade Math",
                GradeLevelRef = 18
            });

            classes.Add(new Class
            {
                Id = 5,
                Name = "Algebra I",
                Description = "Algebra I",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 6,
                Name = "Pre-Algebra",
                Description = "Pre-Algebra",
                GradeLevelRef = 9
            });

            classes.Add(new Class
            {
                Id = 7,
                Name = "MS Earth Sci",
                Description = "Middle School Earth Science",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 8,
                Name = "Lang Arts 6",
                Description = "6th Grade Language Arts",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 9,
                Name = "Wrt Logic I",
                Description = "Writing and Logic I",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 10,
                Name = "MS Hm Ltr I",
                Description = "MS Humane Let I - Acnt/Classic",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 11,
                Name = "Latin II",
                Description = "Latin II",
                GradeLevelRef = 9
            });

            classes.Add(new Class
            {
                Id = 12,
                Name = "Bible NT",
                Description = "Bible - New Testament",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 13,
                Name = "Biology",
                Description = "Biology",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 14,
                Name = "HL Modern I",
                Description = "Humane Letters - Modern I",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 15,
                Name = "Spanish I",
                Description = "Spanish I",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 16,
                Name = "Bib WrldView",
                Description = "Biblical WorldView",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 17,
                Name = "Phy Ed 6",
                Description = "6th Grade Physical Education",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 21,
                Name = "Choir 6",
                Description = "6th Grade Choir",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 27,
                Name = "Art 6",
                Description = "6th Grade Art",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 28,
                Name = "Art I",
                Description = "Art I",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 29,
                Name = "MS Phy Ed I",
                Description = "MS Physical Education I",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 30,
                Name = "Spanish II",
                Description = "Spanish II",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 34,
                Name = "Life Sci",
                Description = "Life Science",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 37,
                Name = "Writ Rhet I",
                Description = "Writing and Rhetoric I",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 38,
                Name = "MS US Hist",
                Description = "Middle School U.S. History",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 39,
                Name = "Latin III",
                Description = "Latin III",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 40,
                Name = "MS Choir I",
                Description = "Middle School Choir I",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 41,
                Name = "Health PE I",
                Description = "Health and Physical Ed. I",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 42,
                Name = "Bible OT1",
                Description = "Bible - Old Testament I",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 43,
                Name = "Geometry",
                Description = "Geometry",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 44,
                Name = "Latin I",
                Description = "Latin I",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 45,
                Name = "MS Art I",
                Description = "Middle School Art I",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 46,
                Name = "Choral I",
                Description = "Choral Perfomance I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 49,
                Name = "Earth Sci",
                Description = "Earth Science",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 86,
                Name = "MS Chapel",
                Description = "Middle School Chapel",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 87,
                Name = "Advsry/Chpl",
                Description = "Advisory Class And Chapel",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 88,
                Name = "Math Team",
                Description = "Math Team",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 89,
                Name = "HS Chapel",
                Description = "High School Chapel",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 90,
                Name = "Activity Per",
                Description = "Activity Period",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 91,
                Name = "Study Hall",
                Description = "Study Hall",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 92,
                Name = "Newspaper",
                Description = "Newspaper",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 93,
                Name = "Yearbook",
                Description = "Yearbook",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 94,
                Name = "Stdnt Leader",
                Description = "Student Counsel/ Peer Jury",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 95,
                Name = "Sports Club",
                Description = "Sports Club",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 96,
                Name = "Gardening",
                Description = "Gardening and Landscaping",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 113,
                Name = "Lunch",
                Description = "Lunch",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 186,
                Name = "Bible OT2",
                Description = "Bible - Old Testament II",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 187,
                Name = "Apologetics",
                Description = "Christian Apologetics",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 188,
                Name = "Theology I",
                Description = "Systematic Theology I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 189,
                Name = "Theology II",
                Description = "Systematic Theology II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 190,
                Name = "Wrt Logic II",
                Description = "Writing and Logic II",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 191,
                Name = "Wrt Rhet II",
                Description = "Writing and Rethoric II",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 192,
                Name = "Spch Rhet I",
                Description = "Speech and Rhetoric I",
                GradeLevelRef = 13
            });

            classes.Add(new Class
            {
                Id = 193,
                Name = "Sr Writing",
                Description = "Senior Writing",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 194,
                Name = "Drama",
                Description = "Drama",
                GradeLevelRef = 13
            });

            classes.Add(new Class
            {
                Id = 195,
                Name = "Sr Thesis",
                Description = "Senior Thesis",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 196,
                Name = "Algebra II",
                Description = "Algebra II",
                GradeLevelRef = 13
            });

            classes.Add(new Class
            {
                Id = 197,
                Name = "Pre-Calculus",
                Description = "Pre-Calculus",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 198,
                Name = "Calculus",
                Description = "Calculus",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 199,
                Name = "Phys Sci",
                Description = "Physical Science",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 200,
                Name = "Chemistry",
                Description = "Chemistry",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 201,
                Name = "Physics",
                Description = "Physics",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 202,
                Name = "MS Hm Ltr II",
                Description = "MS Humane Let II - Medieval",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 203,
                Name = "HL Modern II",
                Description = "Humane Letters - Modern II",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 204,
                Name = "HL Medieval",
                Description = "Humane Letters - Medieval",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 205,
                Name = "HL Anct Clsc",
                Description = "Humane Let-Ancient and Classical",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 206,
                Name = "Spanish III",
                Description = "Spanish III",
                GradeLevelRef = 13
            });

            classes.Add(new Class
            {
                Id = 207,
                Name = "Spanish IV",
                Description = "Spanish IV",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 208,
                Name = "MS Art II",
                Description = "Middle School Art II",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 209,
                Name = "MS Choir II",
                Description = "Middle School Choir II",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 210,
                Name = "Art II",
                Description = "Art II",
                GradeLevelRef = 13
            });

            classes.Add(new Class
            {
                Id = 211,
                Name = "Choral II",
                Description = "Choral Perfomance II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 212,
                Name = "Choral",
                Description = "Choral Perfomance",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 213,
                Name = "Choral",
                Description = "Choral Perfomance",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 214,
                Name = "MS Phy Ed II",
                Description = "MS Physical Education II",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 215,
                Name = "Health PE II",
                Description = "Health and Physical Ed. II",
                GradeLevelRef = 13
            });

            

            classes.Add(new Class
            {
                Id = 257,
                Name = "US History",
                Description = "United States History",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 258,
                Name = "Lang Arts",
                Description = "Language Arts",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 259,
                Name = "Spanish I",
                Description = "Spanish I",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 260,
                Name = "Algebra",
                Description = "Algebra",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 261,
                Name = "Biology",
                Description = "Biology",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 261,
                Name = "Biology",
                Description = "Biology",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 262,
                Name = "Art",
                Description = "Art",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 263,
                Name = "Music",
                Description = "Music",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 264,
                Name = "Phy Ed",
                Description = "Physical Education",
                GradeLevelRef = 11
            });

            classes.Add(new Class
            {
                Id = 344,
                Name = "Art Elect I",
                Description = "Art Elective I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 345,
                Name = "ChoralElct I",
                Description = "Choral Elective I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 346,
                Name = "TechElect I",
                Description = "Technology Elective I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 346,
                Name = "TechElect I",
                Description = "Technology Elective I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 402,
                Name = "Math Support",
                Description = "Math Support",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 403,
                Name = "Read Support",
                Description = "Reading Support",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 404,
                Name = "4B Grades",
                Description = "4B Grades (Ms. Hoilien)",
                GradeLevelRef = 5
            });

            classes.Add(new Class
            {
                Id = 407,
                Name = "Pre-Calc",
                Description = "Pre-Calculus",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 408,
                Name = "Calc An Geom",
                Description = "Calc & Analytic Geom I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 408,
                Name = "Calc An Geom",
                Description = "Calc & Analytic Geom I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 409,
                Name = "Essn of Engl",
                Description = "Essense of English I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 410,
                Name = "Algebra I",
                Description = "Algebra I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 411,
                Name = "Art",
                Description = "Art",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 411,
                Name = "Art",
                Description = "Art",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 412,
                Name = "Intro to Agr",
                Description = "Introduction to Agriculture",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 413,
                Name = "Hlth 1st Aid",
                Description = "Health and First Aid",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 414,
                Name = "Advisory",
                Description = "Advisory",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 415,
                Name = "SC English",
                Description = "SC English",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 416,
                Name = "SC Phy Sci",
                Description = "SC Physical Science",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 416,
                Name = "SC Phy Sci",
                Description = "SC Physical Science",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 417,
                Name = "SC Algebra",
                Description = "SC Algebra",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 418,
                Name = "Phy Ed 9",
                Description = "Physical Education",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 419,
                Name = "SC Geography",
                Description = "SG Geography",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 420,
                Name = "ELL Level II",
                Description = "ELL Level II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 421,
                Name = "SC Phy Sci",
                Description = "SC Physical Science",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 422,
                Name = "SC Algebra",
                Description = "SC Algebra",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 423,
                Name = "ELL Level II",
                Description = "ELL Level II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 424,
                Name = "Nutr Foods",
                Description = "Nutrition and Foods",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 425,
                Name = "MN Aviation",
                Description = "MN Aviation Career Ed Camp",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 426,
                Name = "Spanish 1",
                Description = "Spanish 1",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 427,
                Name = "Callig/Desig",
                Description = "Caligraphy & Design",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 428,
                Name = "English 9",
                Description = "English 9",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 429,
                Name = "Social 9",
                Description = "Social Studies 9",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 430,
                Name = "Algebra 1",
                Description = "Algebra 1",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 431,
                Name = "Physical Sci",
                Description = "Physical Science 9A",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 432,
                Name = "Comm",
                Description = "Communications",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 433,
                Name = "Spanish 1B",
                Description = "Spanish 1B",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 434,
                Name = "English 9B",
                Description = "English 9B",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 435,
                Name = "Social 9B",
                Description = "Social Studies 9B",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 436,
                Name = "Algebra 1B",
                Description = "Algebra 1B",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 437,
                Name = "Physical Sci",
                Description = "Physical Science 9B",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 438,
                Name = "Spanish 1C",
                Description = "Spanish 1C",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 439,
                Name = "English 9C",
                Description = "English 9C",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 440,
                Name = "Jewelry",
                Description = "Jewelry",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 441,
                Name = "Social 9C",
                Description = "Social Studies 9C",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 442,
                Name = "Algebra 1C",
                Description = "Algebra 1C",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 443,
                Name = "Physical Sci",
                Description = "Physical Science 9B",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 444,
                Name = "French 1A",
                Description = "French 1A",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 445,
                Name = "Phy Ed 9A",
                Description = "Physical Education 9A",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 446,
                Name = "French 1B",
                Description = "French 1B",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 447,
                Name = "Phy Ed 9B",
                Description = "Physical Education 9B",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 448,
                Name = "Des. / Prod.",
                Description = "Design & Production",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 449,
                Name = "French 1C",
                Description = "French 1C",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 450,
                Name = "Humanities 2",
                Description = "Humanities 2",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 451,
                Name = "AP Hum. 2",
                Description = "AP Humanities 2",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 452,
                Name = "Chinese L1",
                Description = "Chinese Level 1",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 453,
                Name = "Algebra 1",
                Description = "Algebra 1",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 454,
                Name = "Chemistry",
                Description = "Chemistry",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 455,
                Name = "Ceramics",
                Description = "Ceramics",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 456,
                Name = "Digital Phot",
                Description = "Digital Photography",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 457,
                Name = "OT Archaelg.",
                Description = "Old Testament Archaelogy",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 458,
                Name = "Hst West Civ",
                Description = "history Western Civilization",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 459,
                Name = "THAILAND",
                Description = "Thailand - Thoen Wittaya Schl.",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 460,
                Name = "Calc AnGe II",
                Description = "Calc & Analytic Geom II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 461,
                Name = "World Relig.",
                Description = "World Religions",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 462,
                Name = "Bs Comp App",
                Description = "Basic Computer Applications",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 463,
                Name = "Fitness Wlns",
                Description = "Lifetime Fitness & Wellness",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 464,
                Name = "Intro Psych",
                Description = "Introduction to Psychology",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 465,
                Name = "Comp I",
                Description = "Composition I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 466,
                Name = "Comp II",
                Description = "Composition II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 467,
                Name = "Beg Span I",
                Description = "Beginning Spanish I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 468,
                Name = "Hist Church",
                Description = "Hist Chr Church Apos to Pres",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 597,
                Name = "Art Elect II",
                Description = "Art Elective II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 598,
                Name = "ChoirElct II",
                Description = "Choir Elective II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 599,
                Name = "TechElect II",
                Description = "Technology and Computers II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 610,
                Name = "Pub Speaking",
                Description = "Public Speaking",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 611,
                Name = "Wght Train",
                Description = "Weight Training",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 612,
                Name = "US HISTORY",
                Description = "United States History",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 613,
                Name = "PsnlFinac I",
                Description = "Personal Finance I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 712,
                Name = "Music 6",
                Description = "6th Grade Music Theory",
                GradeLevelRef = 8
            });

            classes.Add(new Class
            {
                Id = 713,
                Name = "Music I",
                Description = "Music History and Theory",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 714,
                Name = "Music II",
                Description = "Music History and Theory",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 714,
                Name = "Music II",
                Description = "Music History and Theory",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 715,
                Name = "PsnlFinac II",
                Description = "Personal Finance II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 716,
                Name = "MusicElect I",
                Description = "Music Elective I",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 717,
                Name = "MusicElct II",
                Description = "Music Elective II",
                GradeLevelRef = 14
            });

            classes.Add(new Class
            {
                Id = 767,
                Name = "MusicElct I",
                Description = "Music Elective I",
                GradeLevelRef = 14
            });


            classes.Add(new Class
            {
                Id = 771,
                Name = "MS Music I",
                Description = "Ms Music History and Theory",
                GradeLevelRef = 10
            });

            classes.Add(new Class
            {
                Id = 814,
                Name = "Eng Comp",
                Description = "English Composition",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 815,
                Name = "Eng Comp",
                Description = "English Composition",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 816,
                Name = "Eng Comp",
                Description = "English Composition",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 817,
                Name = "Fnd Faith",
                Description = "Foundations of Faith",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 818,
                Name = "Fnd Faith",
                Description = "Foundations of Faith",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 819,
                Name = "Fnd Faith",
                Description = "Foundations of Faith",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 820,
                Name = "Geography",
                Description = "Geography",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 821,
                Name = "Geography",
                Description = "Geography",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 822,
                Name = "Geography",
                Description = "Geography",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 823,
                Name = "Algebra 1",
                Description = "Algebra 1",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 824,
                Name = "Algebra 1",
                Description = "Algebra 1",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 825,
                Name = "Algebra 1",
                Description = "Algebra 1",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 826,
                Name = "Phys Sci",
                Description = "Physical Science",
                GradeLevelRef = 12
            });

            classes.Add(new Class
            {
                Id = 827,
                Name = "Phys Sci",
                Description = "Physical Science",
                GradeLevelRef = 12
            });


            Add(classes);
        }

        public ClassQueryResult GetClassesComplex(ClassQuery query)
        {
            throw new NotImplementedException();
            /*var parameters = new Dictionary<string, object>
                {
                    {SCHOOL_YEAR_ID_PARAM, query.SchoolYearId},
                    {MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId},
                    {PERSON_ID_PARAM, query.PersonId},
                    {CLASS_ID_PARAM, query.ClassId},
                    {CALLER_ID_PARAM, query.CallerId},
                    {START_PARAM, query.Start},
                    {COUNT_PARAM, query.Count},
                    {CALLER_ROLE_ID_PARAM, query.CallerRoleId},
                    {SCHOOL_ID, schoolId}
                };

            string filter1 = null;
            string filter2 = null;
            string filter3 = null;
            if (!string.IsNullOrEmpty(query.Filter))
            {
                string[] sl = query.Filter.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sl.Length > 0)
                    filter1 = string.Format(FILTER_FORMAT, sl[0]);
                if (sl.Length > 1)
                    filter2 = string.Format(FILTER_FORMAT, sl[1]);
                if (sl.Length > 2)
                    filter3 = string.Format(FILTER_FORMAT, sl[2]);
            }
            parameters.Add(FILTER1_PARAM, filter1);
            parameters.Add(FILTER2_PARAM, filter2);
            parameters.Add(FILTER3_PARAM, filter3);


            using (var reader = ExecuteStoredProcedureReader(GET_CLASSES_PROC, parameters))
            {
                var sourceCount = reader.Read() ? SqlTools.ReadInt32(reader, "SourceCount") : 0;
                reader.NextResult();
                var classes = new List<ClassDetails>();
                while (reader.Read())
                {
                    var c = reader.Read<ClassDetails>(true);
                    if (c.TeacherRef.HasValue)
                        c.Teacher = reader.Read<Person>(true);
                    c.GradeLevel = reader.Read<GradeLevel>(true);
                    classes.Add(c);
                }
                reader.NextResult();
                var markingPeriodClasses = reader.ReadList<MarkingPeriodClass>();
                foreach (var classComplex in classes)
                {
                    classComplex.MarkingPeriodClasses = markingPeriodClasses.Where(x => x.ClassRef == classComplex.Id).ToList();
                }
                return new ClassQueryResult { Classes = classes, Query = query, SourceCount = sourceCount };
             * 
             * var classes = data.Select(x => x.Value).ToList();
            var result = new ClassQueryResult
            {
                Classes = classes,
                Query = query,
                SourceCount = classes.Count
            }
            return 
            }*/

        }
    }
}
