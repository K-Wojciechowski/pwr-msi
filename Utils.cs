namespace pwr_msi {
    public static class Utils {
        public static int? TryParseInt(string s) {
            int i;
            var res = int.TryParse(s, out i);
            return res ? i : null;
        }
    }
}
