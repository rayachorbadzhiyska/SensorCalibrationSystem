﻿using System.Collections.Generic;

namespace SensorCalibrationSystem.Models
{
    /// <summary>
    /// Represents a memory map.
    /// </summary>
    public class MemoryMapModel
    {
        /// <summary>
        /// Gets or sets which sensor the memory map belongs to.
        /// </summary>
        public string Sensor { get; set; }

        /// <summary>
        /// Gets or sets the path to the memory map's image.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the memory map's registers.
        /// </summary>
        public List<RegisterModel> Registers { get; set; }
    }
}
