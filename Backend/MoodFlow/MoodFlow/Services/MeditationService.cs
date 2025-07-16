using MoodFlow.Data;
using MoodFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace MoodFlow.Services
{
    public interface IMeditationService
    {
        Task<IEnumerable<object>> GetExercisesAsync(int? userId);
        Task<bool> ToggleFavouriteAsync(int userId, int exerciseId);
        Task<IEnumerable<MeditationExercise>> GetFavouritesAsync(int userId);
        Task<bool> IsFavouriteAsync(int userId, int exerciseId);
    }

    public class MeditationService : IMeditationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MeditationService> _logger;

        public MeditationService(ApplicationDbContext context, ILogger<MeditationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<object>> GetExercisesAsync(int? userId)
        {
            try
            {
                var exercises = await _context.MeditationExercises.ToListAsync();

                if (userId.HasValue)
                {
                    var userFavourites = await _context.UserMeditationExercises
                        .Where(ume => ume.UserId == userId.Value)
                        .Select(ume => ume.MeditationExerciseId)
                        .ToListAsync();

                    var exercisesWithFavourites = exercises.Select(exercise => new
                    {
                        exercise.Id,
                        exercise.Title,
                        exercise.VideoUrl,
                        exercise.Description,
                        IsFavourite = userFavourites.Contains(exercise.Id)
                    });

                    return exercisesWithFavourites;
                }
                else
                {
                    var exercisesWithoutFavourites = exercises.Select(exercise => new
                    {
                        exercise.Id,
                        exercise.Title,
                        exercise.VideoUrl,
                        exercise.Description,
                        IsFavourite = false
                    });

                    return exercisesWithoutFavourites;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching meditation exercises");
                throw;
            }
        }

        public async Task<bool> ToggleFavouriteAsync(int userId, int exerciseId)
        {
            try
            {
                var exercise = await _context.MeditationExercises.FindAsync(exerciseId);
                if (exercise == null)
                {
                    throw new ArgumentException("Meditation exercise not found");
                }

                var existingFavourite = await _context.UserMeditationExercises
                    .FirstOrDefaultAsync(ume => ume.UserId == userId && ume.MeditationExerciseId == exerciseId);

                if (existingFavourite != null)
                {
                    _context.UserMeditationExercises.Remove(existingFavourite);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Removed meditation exercise {exerciseId} from favourites for user {userId}");
                    return false;
                }
                else
                {
                    var newFavourite = new UserMeditationExercise
                    {
                        UserId = userId,
                        MeditationExerciseId = exerciseId
                    };
                    _context.UserMeditationExercises.Add(newFavourite);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Added meditation exercise {exerciseId} to favourites for user {userId}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling meditation exercise favourite");
                throw;
            }
        }

        public async Task<IEnumerable<MeditationExercise>> GetFavouritesAsync(int userId)
        {
            try
            {
                var favouriteExercises = await _context.UserMeditationExercises
                    .Where(ume => ume.UserId == userId)
                    .Join(_context.MeditationExercises,
                          ume => ume.MeditationExerciseId,
                          me => me.Id,
                          (ume, me) => me)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {favouriteExercises.Count} favourite meditation exercises for user {userId}");
                return favouriteExercises;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching favourite meditation exercises");
                throw;
            }
        }

        public async Task<bool> IsFavouriteAsync(int userId, int exerciseId)
        {
            try
            {
                var isFavourite = await _context.UserMeditationExercises
                    .AnyAsync(ume => ume.UserId == userId && ume.MeditationExerciseId == exerciseId);

                return isFavourite;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if meditation exercise is favourite");
                throw;
            }
        }
    }
} 