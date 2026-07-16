using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Systems
{
    public class AccountPermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } = 1; // Tek satır olacağı için ID'yi 1'e sabitleyip kapatıyoruz.

        public bool CanRegister { get; set; } = true;

        public bool CanResetPassword { get; set; } = true;

        public bool CanLogin { get; set; } = true;
        public bool WebActionInfos { get; set; } = true;
    }
}