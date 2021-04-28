using System;
using System.Collections.Generic;
using System.Linq;
using Dishes.Interfaces;
using Dishes.Models;
using Dishes.Repositories;

namespace Dishes.Services
{
    public class Service
    {
        private readonly DishRepository _dishRepository;
        private readonly SourceRepository _sourceRepository;
        private readonly TagRepository _tagRepository;
        public List<IDbEntity> Sources { get; private set; }
        public List<IDbEntity> Dishes { get; private set; }
        public List<IDbEntity> Tags { get; set; }


        public Service()
        {
            var connectionString = $"Data Source={AppSettingsService.Instance.ConnectionString}";
            var dbMigrationService = new DbMigrationService(connectionString);
            dbMigrationService.VerifySchema();
            _dishRepository = new DishRepository(connectionString);
            Dishes = new List<IDbEntity>();
            _sourceRepository = new SourceRepository(connectionString);
            Sources = new List<IDbEntity>();
            _tagRepository = new TagRepository(connectionString);
            Tags = new List<IDbEntity>();
        }

        public void LoadAll()
        {
            Dishes = _dishRepository.LoadDishes();
            Sources = _sourceRepository.LoadSources();
            Tags = _tagRepository.LoadTags();
            var dishTags = _dishRepository.LoadDishTags();
            SortDishes();
            SortSources();
            SortTags();
            foreach (var dbEntity in Dishes)
            {
                var dish = (Dish) dbEntity;
                dish.Source = (Source)Sources.First(s => s.Id == dish.SourceId);
                dish.Tags = dishTags
                    .Where(dt => dt.DishId == dish.Id)
                    .Select(dt => (Tag)Tags.First(t => t.Id == dt.TagId))
                    .ToList();
            }
        }

        private void SortSources()
        {
            Sources.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        private void SortTags()
        {
            Tags.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        private void SortDishes()
        {
            Dishes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        public void AddDish(Dish dish)
        {
            _dishRepository.AddDish(dish);
            Dishes.Add(dish);
            SortDishes();
        }

        public void AddSource(Source source)
        {
            _sourceRepository.AddSource(source);
            Sources.Add(source);
            SortSources();
        }

        public void AddTag(Tag tag)
        {
            _tagRepository.AddTag(tag);
            Tags.Add(tag);
            SortTags();
        }

        public void UpdateDish(Dish dish)
        {
            _dishRepository.UpdateDish(dish);
            SortDishes();
        }

        public void UpdateSource(Source source)
        {
            _sourceRepository.UpdateSource(source);
            SortSources();
        }

        public void UpdateTag(Tag tag)
        {
            _tagRepository.UpdateTag(tag);
            SortTags();
        }

        public void DeleteDish(int dishId)
        {
            _dishRepository.DeleteDish(dishId);
            Dishes.Remove(Dishes.First(dish => dish.Id == dishId));
        }

        public void DeleteSource(int sourceId)
        {
            _sourceRepository.DeleteSource(sourceId);
            Sources.Remove(Sources.First(source => source.Id == sourceId));
        }

        public void DeleteTag(int tagId)
        {
            _tagRepository.DeleteTag(tagId);
            Tags.Remove(Tags.First(tag => tag.Id == tagId));
        }
    }
}