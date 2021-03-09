using System;
using System.Collections.Generic;
using System.Linq;

namespace Dishes
{
    public class Service
    {
        private readonly DishRepository _dishRepository;
        private readonly SourceRepository _sourceRepository;
        public List<Source> Sources { get; private set; }
        public List<Dish> Dishes { get; private set; }

        public Service()
        {
            var connectionString = @"Data Source=C:\Users\zacha\Desktop\New folder\dishes.db";
            _dishRepository = new DishRepository(connectionString);
            Dishes = new List<Dish>();
            _sourceRepository = new SourceRepository(connectionString);
            Sources = new List<Source>();
        }

        public void LoadAll()
        {
            Dishes = _dishRepository.LoadDishes();
            Sources = _sourceRepository.LoadSources();
            SortDishes();
            SortSources();
            foreach (var dish in Dishes)
            {
                dish.Source = Sources.First(s => s.Id == dish.SourceId);
            }
        }

        private void SortSources()
        {
            Sources.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
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
    }
}