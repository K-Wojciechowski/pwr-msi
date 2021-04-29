namespace pwr_msi.Models.Dto {
    public class ResultDto<T> {
        public T Result { get; set; }

        public ResultDto(T result) {
            Result = result;
        }
    }
}
