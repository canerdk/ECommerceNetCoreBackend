using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class LanguageManager : ILanguageService
    {
        private readonly ILanguageDal _languageDal;
        public LanguageManager(ILanguageDal languageDal)
        {
            _languageDal = languageDal;
        }

        public List<Language> GetAll()
        {
            return _languageDal.GetAll();
        }
    }
}
