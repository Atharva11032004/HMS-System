using Microsoft.EntityFrameworkCore;
using RoomService.Data;
using RoomService.Models;

namespace RoomService.Services
{
    public class RoomBusinessService
    {
        private readonly ApplicationDbContext _context;

        public RoomBusinessService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<RoomDto>> GetAvailableRoomsAsync(
            DateTime checkIn,
            DateTime checkOut,
            int adults,
            int children)
        {
            if (checkIn >= checkOut)
                throw new ArgumentException("Check-in must be earlier than check-out.");

            var totalGuests = adults + children;

            
            var blockedRoomIds = await _context.RoomCalendars
                .Where(rc =>
                    rc.Status == "Blocked" &&
                    checkIn < rc.EndDate &&
                    checkOut > rc.StartDate)
                .Select(rc => rc.RoomId)
                .Distinct()
                .ToListAsync();

            var availableRooms = await _context.Rooms
                .Include(r => r.RoomType)
                .Where(r =>
                    !blockedRoomIds.Contains(r.Id) &&
                    r.RoomType!.MaxOccupancy >= totalGuests)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomTypeName = r.RoomType!.Name,
                    IsAvailable = true
                })
                .ToListAsync();

            return availableRooms;
        }

       
        public async Task BlockRoomAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
                throw new ArgumentException("Check-in date must be earlier than check-out date.");

           
            bool roomExists = await _context.Rooms.AnyAsync(r => r.Id == roomId);
            if (!roomExists)
                throw new KeyNotFoundException($"Room ID {roomId} does not exist.");

          
            bool hasOverlap = await _context.RoomCalendars
                .AnyAsync(rc =>
                    rc.RoomId == roomId &&
                    rc.Status == "Blocked" &&
                    checkIn < rc.EndDate &&
                    checkOut > rc.StartDate);

            if (hasOverlap)
                throw new InvalidOperationException("Room is already blocked for the selected dates.");

            var calendar = new RoomCalendar
            {
                RoomId = roomId,
                StartDate = checkIn,
                EndDate = checkOut,
                Status = "Blocked"
            };

            _context.RoomCalendars.Add(calendar);
            await _context.SaveChangesAsync();
        }

 
        public async Task FreeRoomAsync(int roomId)
        {
            var calendar = await _context.RoomCalendars
                .Where(rc =>
                    rc.RoomId == roomId &&
                    rc.Status == "Blocked")
                .OrderByDescending(rc => rc.EndDate)
                .FirstOrDefaultAsync();

            if (calendar != null)
            {
              
                calendar.Status = "Released"; 
                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<RoomDto>> GetRoomsAsync()
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomTypeName = r.RoomType!.Name,
                    IsAvailable = r.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<RoomDto?> GetRoomAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return null;

            return new RoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomTypeName = room.RoomType!.Name,
                IsAvailable = room.IsAvailable
            };
        }

        public async Task<RoomDto> CreateRoomAsync(Room room)
        {
            bool typeExists = await _context.RoomTypes
                .AnyAsync(rt => rt.Id == room.RoomTypeId);

            if (!typeExists)
                throw new KeyNotFoundException("RoomType does not exist.");

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var rt = await _context.RoomTypes.FindAsync(room.RoomTypeId);

            return new RoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomTypeName = rt!.Name,
                IsAvailable = room.IsAvailable
            };
        }

        public async Task<bool> UpdateRoomAsync(int id, Room room)
        {
            var existing = await _context.Rooms.FindAsync(id);
            if (existing == null) return false;

         
            bool typeExists = await _context.RoomTypes
                .AnyAsync(rt => rt.Id == room.RoomTypeId);

            if (!typeExists)
                throw new KeyNotFoundException($"RoomType with ID {room.RoomTypeId} does not exist.");

            existing.RoomNumber = room.RoomNumber;
            existing.RoomTypeId = room.RoomTypeId;
            existing.IsAvailable = room.IsAvailable;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

    
        public async Task<List<RoomTypeDto>> GetRoomTypesAsync()
        {
            return await _context.RoomTypes
                .Select(rt => new RoomTypeDto
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    Description = rt.Description,
                    MaxOccupancy = rt.MaxOccupancy
                })
                .ToListAsync();
        }

        public async Task<RoomTypeDto?> GetRoomTypeAsync(int id)
        {
            var rt = await _context.RoomTypes.FindAsync(id);
            if (rt == null) return null;

            return new RoomTypeDto
            {
                Id = rt.Id,
                Name = rt.Name,
                Description = rt.Description,
                MaxOccupancy = rt.MaxOccupancy
            };
        }

        public async Task<RoomTypeDto> CreateRoomTypeAsync(RoomType roomType)
        {
            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();

            return new RoomTypeDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                MaxOccupancy = roomType.MaxOccupancy
            };
        }

        public async Task<bool> UpdateRoomTypeAsync(int id, RoomType roomType)
        {
            var existing = await _context.RoomTypes.FindAsync(id);
            if (existing == null) return false;

            existing.Name = roomType.Name;
            existing.Description = roomType.Description;
            existing.MaxOccupancy = roomType.MaxOccupancy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoomTypeAsync(int id)
        {
            var rt = await _context.RoomTypes.FindAsync(id);
            if (rt == null) return false;

            _context.RoomTypes.Remove(rt);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}