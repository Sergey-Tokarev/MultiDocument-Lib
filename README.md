# MultiDocument-Lib
                                                           Multi Document
Introduction

MultiDocument is a reusable library which allows reading, modification and conversion of different file formats. This library allows adding support for new user supplied file formats. MultiDocument is a very flexible and extensible library.
One of the core principles of MultiDocument library is possibility of extension functionality without recompiling source code. This extensibility is achieved by using an Abstract Factory and Builder patterns. Even though some basic formats (e.g. “binary” and “xml”) are supported by default, their functionality are also provided via factories, so you can extend functionality by providing your own factory which can either provide its own logic or provide logic which extends existing one.

Architecture

The basic class of MultiDocument library is MDocument<T>. You can use this class as some kind of generic container (like List<T>). It implements IMReader<T>, IMWriter<T> and IMStream interfaces. IMReader<T> interface provides functionality for reading operations, IMWriter<T> provides functionality for writing operations and IMStream provides functionality for getting count of elements.

First at all you should understand that MDocument<T> is not the only way you could get functionality of IMReader<T>, IMWriter<T> and IMStream interfaces. There are two basic classes for both reading and writing operations: MBinaryReader<T>, MXmlReader<T>, MBinaryWriter<T> and MXmlWriter<T>. As long as this classes are also implements those interfaces you can use them separately or in conjunction with other library classes to get more flexibility. 
As you can see all these classes are generic classes. The template parameter T represents a type of record that will be saved in the target file. It can be structure or class. The main goal of using this library is that you can select only some bunch of fields and properties you want to be used (serialized) in serialization process. To do this you should mark each field and property you want to be serialized with ProcessableAttribute attribute. It gives you a very flexible way for managing of creation of file format by specifying the record format. So you do not have to hard-code which field should be serialized at reader-writer level. The library lets you do this on the user level while you’re defining a class of record. Otherwise we had to write new classes of reader and writer on each new record.

By setting up different properties of ProcessableAttribute attribute you can tune the process of serialization: 
1.	setting up the alias for field or property (it's useful for XML format) - Alias property; 
2.	setting up the rank for field or property (so you can specify the sequence at which the specified properties will be serialized) - Rank property; 
3.	setting up the serialization factory for field or property (so this factory will be used by library when it need to serialize/deserialize specified field or property) - SerializerFactory property; 
4.	setting up the data verifier (when this property is set, the library will use it to verify specified property or field) - DataVerifier property.

NOTE.  It’s important to mark the properties and fields you want to be serialized with ProcessableAttribute attribute if you use standard classes (like MBinaryReader<T>, MXmlReader<T>, MBinaryWriter<T> and MXmlWriter<T>).  Also note that these standard classes are used internally by MDocument<T> class.
Also note that it’s not required specify this attribute for records if your custom classes that implement IMReader<T> and  IMWriter<T>  interfaces do not use this attribute in their logic. 

The library also provide functionality for data converting. All classes in library, that use converters (MDocument<T>, MBinaryReader<T> and MXmlReader<T>) use converters that implement IDataConverter<T> interface. The MBinaryReader<T> and MXmlReader<T> classes have in their constructor parameter converter of type IDataConverter<T> which is by default has null value. You can specify a concrete converter in constructor of one of these readers, so you can then call ConvertDocument() method to convert all reader data using specified converter. So, all default readers (MBinaryReader<T> and MXmlReader<T>) implement Builder pattern for conversion purposes. There are two basic data converter: MBinaryConverter<T> and MXmlConverter<T>. They can be used in conjunction with readers as implementation of Builder pattern or they can be used as separate data converters.  

