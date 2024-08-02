using CommunityToolkit.Mvvm.ComponentModel;

namespace SensorCalibrationSystem.Models
{
    /// <summary>
    /// Represents a memory map register.
    /// </summary>
    public class RegisterModel : ObservableObject
    {
        private string? value;

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
        /// Gets or sets the register's size in bits.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the register's access.
        /// </summary>
        public string Access { get; set; }

        /// <summary>
        /// Gets or sets the register's value.
        /// </summary>
        public string? Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }

        public RegisterModel(string name, string description, string address, string access)
        {
            Name = name;
            Description = description;
            Address = address;
            Access = access;
            Value = "0";
        }
    }
}
