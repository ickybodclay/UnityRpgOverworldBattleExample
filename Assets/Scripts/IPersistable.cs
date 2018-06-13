public interface IPersistable<T> {
    void SaveData();
    void LoadData(T data);
}
