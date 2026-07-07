using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.App
{
    public class UserDevice
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string DeviceId { get; private set; } = null!;

        public DateTime? AssignDate { get; private set; }

        public bool IsReleased { get; private set; }

        public DateTime? ReleaseDate { get; private set; }

        public Guid? ReleaseBy { get; private set; }

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
