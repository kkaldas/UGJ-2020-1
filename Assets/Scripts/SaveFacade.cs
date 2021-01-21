﻿public class SaveFacade
{
    private MapsPersistence mapsPersistence = new MapsPersistence();
    private ClassesPersistence classesPersistence = new ClassesPersistence();
    private DeckPersistence deckPersistence = new DeckPersistence("FirstDeck");
    private DeckPersistence cardsCollectionPersistence = new DeckPersistence("CardsCollection");
    private DeckPersistence cardsLevelPersistence = new DeckPersistence("CardsLevel");

    // Needed for saving.
    private static string[] nameOfMapsToSave;
    private static MapSerializable[] dataOfMapsToSave;
    private static ClassesSerializable classesSerializableToSave;
    private static DeckSerializable deckSerializableToSave;
    private static DeckSerializable cardsCollectionToSave;
    private static DeckSerializable cardsLevelToSave;

    // Needed for loading.
    private static string[] namesOfMapsToLoad;

    // Results of loading
    private static MapSerializable[] loadedMapsInfo;
    private static ClassesSerializable loadedClassesSerializable;
    private static DeckSerializable loadedDeckSerializable;
    private static DeckSerializable loadedCardsCollection;
    private static DeckSerializable loadedCardsLevel;

    public bool DoesAnySaveExist()
    {
        return mapsPersistence.DoesMapSaveExist("First");
    }

    public void PrepareMapsForSaving(string[] mapNames, MapSerializable[] mapsInfo)
    {
        nameOfMapsToSave = mapNames;
        dataOfMapsToSave = mapsInfo;
    }
    public void PrepareClassesBonusesForSaving( ClassesSerializable classesInfo )
    {
        classesSerializableToSave = classesInfo;
    }
    public void PrepareDeckForSaving(DeckSerializable deckSerializableParam)
    {
        deckSerializableToSave = deckSerializableParam;
        // Because DeckPrototypeFactory checks if deck is loaded. And from this moment on we can say it is.
        loadedDeckSerializable = deckSerializableParam;
    }
    public void PrepareCardsCollectionForSaving(DeckSerializable cardsCollectionSerializableParam)
    {
        cardsCollectionToSave = cardsCollectionSerializableParam;
        loadedCardsCollection = cardsCollectionSerializableParam;
    }
    public void PrepareCardsLevelForSaving(DeckSerializable cardsLevelSerializableParam)
    {
        cardsLevelToSave = cardsLevelSerializableParam;
        loadedCardsLevel = cardsLevelSerializableParam;
    }

    public void PrepareMapsForLoading(string[] mapNames)
    {
        namesOfMapsToLoad = mapNames;
    }

    public void LoadEverything()
    {
        if (DoesAnySaveExist())
        {
            loadedMapsInfo = mapsPersistence.LoadAllMaps(namesOfMapsToLoad);
            loadedClassesSerializable = classesPersistence.LoadClasses();
            loadedDeckSerializable = deckPersistence.Load();
            loadedCardsCollection = cardsCollectionPersistence.Load();
            loadedCardsLevel = cardsLevelPersistence.Load();

            classesSerializableToSave = loadedClassesSerializable;
            deckSerializableToSave = loadedDeckSerializable;
            cardsLevelToSave = loadedCardsLevel;
        }
        else
        {
            L.ogError("LoadEverything was called, but any save exist on storage", this);
        }
    }
    
    public MapSerializable[] GetLoadedMapsInfo()
    {
        if (loadedMapsInfo == null)
        {
            L.ogError("loadedMapsInfo is null!! LoadAll method should be called first!", this);
        }
        return loadedMapsInfo;
    }
    public ClassesSerializable GetLoadedClasses()
    {
        if (loadedClassesSerializable == null)
        {
            L.ogError("loadedMapsInfo is null!! LoadAll method should be called first!", this);
        }
        return loadedClassesSerializable;
    }
    public DeckSerializable GetLoadedDeck()
    {
        return loadedDeckSerializable;
    }
    public DeckSerializable GetLoadedCardsCollection()
    {
        return loadedCardsCollection;
    }
    public DeckSerializable GetLoadedCardsLevel()
    {
        return loadedCardsLevel;
    }

    public bool IsDeckLoaded()
    {
        return loadedDeckSerializable != null;
    }
    public bool IsCardsCollectionLoaded()
    {
        return loadedCardsCollection != null;
    }
    public void ClearCardsCollection()
    {
        loadedCardsCollection = null;
        cardsCollectionToSave = null;
    }

    public void SaveEverything()
    {
        if ( SafeToSave() )
        {
            mapsPersistence.SaveAllMaps(nameOfMapsToSave, dataOfMapsToSave);
            classesPersistence.SaveClasses(classesSerializableToSave);
            deckPersistence.Save(deckSerializableToSave);
            if (cardsCollectionToSave == null)
            {
                cardsCollectionToSave = new DeckSerializable(CardsCollection.GetCardsCollectionAmounts());
            }
            cardsCollectionPersistence.Save(cardsCollectionToSave);
            cardsLevelPersistence.Save(cardsLevelToSave);
        }
        else
        {
            string whatsnull = ("maps: "+dataOfMapsToSave + "mapName: "+nameOfMapsToSave + "classes: "+classesSerializableToSave 
                + "deck: "+deckSerializableToSave + " levels: "+cardsLevelToSave);
            L.ogWarning("SaveEverything was called, but at least one attribute is still null. " +
                "This is ok if there is nothing to save.\n"+whatsnull, this);
        }
    }
    private bool SafeToSave()
    {
        return  dataOfMapsToSave != null
                && nameOfMapsToSave != null
                && classesSerializableToSave != null
                && deckSerializableToSave != null
                && cardsLevelToSave != null
                // cardsCollectionToSave is optional
                ;
    }
}
