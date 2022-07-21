using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Flexpage.Models
{
    public class PlayerModel
    {
        public string Title { get; set; }
        public string ThumbUrl { get; set; }
        public string MediaUrl { get; set; }
        public bool DisplayTitle { get; set; }
        public bool AutoPlay { get; set; }
        public bool Loop { get; set; }
        public bool DisplayControls { get; set; } = true;
        public string Color { get; set; }
        public short Opacity { get; set; }
        public bool ApplySizeConstraints { get; set; } = false;
        public DimensionModel Width { get; set; }
        public DimensionModel Height { get; set; }
        public MediaType MediaType { get; set; }
        public PlayerModel()
        {
        }

        public PlayerModel(MediaModel source)
        {
            ApplySizeConstraints = source.ApplySizeConstraints;
            AutoPlay = source.AutoPlay;
            Color = source.Color;
            DisplayControls = source.DisplayControls;
            DisplayTitle = source.DisplayTitle;
            Height = source.Height;
            Width = source.Width;
            Loop = source.Loop;
            Opacity = source.Opacity;
            ThumbUrl = source.NonEmptyLocalization != null ? source.NonEmptyLocalization.ThumbUrl : "";
            Title = source.Title.Current;
            MediaUrl = source.NonEmptyLocalization != null ? source.NonEmptyLocalization.MediaUrl : "";
            MediaType= source.MediaType;
        }

    }
}