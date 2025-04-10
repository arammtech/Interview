﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.DTOs
{
    public class GenAIApiResponse
    {
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
    }

    public class Content
    {
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        public string Text { get; set; }
    }
}
