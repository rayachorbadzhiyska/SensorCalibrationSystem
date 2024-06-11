﻿namespace SensorCalibrationSystem.Models
{
    /// <summary>
    /// Represents a memory map register.
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Gets or sets the register's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the register's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the register's address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the register's access.
        /// </summary>
        public string Access { get; set; }

        /// <summary>
        /// Gets or sets the register's value.
        /// </summary>
        public int Value { get; set; }
    }
}
