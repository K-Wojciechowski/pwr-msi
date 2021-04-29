export abstract class ObjectWrapper<T> {
    public v: T;
    public constructor(value: T) {
        this.v = value;
    }

}
