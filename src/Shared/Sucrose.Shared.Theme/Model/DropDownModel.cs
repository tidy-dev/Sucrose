﻿using Newtonsoft.Json;
using SSTMCM = Sucrose.Shared.Theme.Model.ControlModel;

namespace Sucrose.Shared.Theme.Model
{
    public class DropDownModel : SSTMCM
    {
        [JsonProperty("value", Required = Required.Always)]
        public int Value { get; set; }

        [JsonProperty("items", Required = Required.Always)]
        public string[] Items { get; set; }

        public DropDownModel() : base("dropdown") { }
    }
}