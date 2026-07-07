using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.App
{
    public class UserDevice
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string DeviceId { get; set; } = null!;

        public DateTime? AssignDate { get; set; }

        public bool IsReleased { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public Guid? ReleaseBy { get; set; }

        private UserDevice() { }

        public UserDevice (Guid id, Guid userId, string deviceId, DateTime? releaseDate, bool isReleased, DateTime? assignDate, Guid? releasedBy)
        {
            Id = id;
            UserId = userId;
            DeviceId = deviceId;
            IsReleased = isReleased;
            AssignDate = assignDate;
            ReleaseBy = releasedBy;
            ReleaseDate = releaseDate;
        }

        public static UserDevice Create(Guid id, Guid userId, string deviceId)
        {
            var userDevice = new UserDevice
            {
                Id = id,
                UserId = userId,
                DeviceId = deviceId,
                AssignDate = DateTime.Now
            };

            return userDevice;
        }
    }
}
