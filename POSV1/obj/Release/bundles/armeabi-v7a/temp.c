/* This source code was produced by mkbundle, do not edit */

#ifndef NULL
#define NULL (void *)0
#endif

typedef struct {
	const char *name;
	const unsigned char *data;
	const unsigned int size;
} MonoBundledAssembly;
void          mono_register_bundled_assemblies (const MonoBundledAssembly **assemblies);
void          mono_register_config_for_assembly (const char* assembly_name, const char* config_xml);

typedef struct _compressed_data {
	MonoBundledAssembly assembly;
	int compressed_size;
} CompressedAssembly;

extern const unsigned char assembly_data_POSV1_dll [];
static CompressedAssembly assembly_bundle_POSV1_dll = {{"POSV1.dll", assembly_data_POSV1_dll, 215552}, 67162};
extern const unsigned char assembly_data_Java_Interop_dll [];
static CompressedAssembly assembly_bundle_Java_Interop_dll = {{"Java.Interop.dll", assembly_data_Java_Interop_dll, 92160}, 27416};
extern const unsigned char assembly_data_Refractored_FloatingActionButton_dll [];
static CompressedAssembly assembly_bundle_Refractored_FloatingActionButton_dll = {{"Refractored.FloatingActionButton.dll", assembly_data_Refractored_FloatingActionButton_dll, 19456}, 8179};
extern const unsigned char assembly_data_System_Data_Portable_dll [];
static CompressedAssembly assembly_bundle_System_Data_Portable_dll = {{"System.Data.Portable.dll", assembly_data_System_Data_Portable_dll, 14336}, 4103};
extern const unsigned char assembly_data_System_Transactions_Portable_dll [];
static CompressedAssembly assembly_bundle_System_Transactions_Portable_dll = {{"System.Transactions.Portable.dll", assembly_data_System_Transactions_Portable_dll, 12800}, 3493};
extern const unsigned char assembly_data_Xamarin_Android_Support_Design_dll [];
static CompressedAssembly assembly_bundle_Xamarin_Android_Support_Design_dll = {{"Xamarin.Android.Support.Design.dll", assembly_data_Xamarin_Android_Support_Design_dll, 315904}, 75462};
extern const unsigned char assembly_data_Xamarin_Android_Support_v4_dll [];
static CompressedAssembly assembly_bundle_Xamarin_Android_Support_v4_dll = {{"Xamarin.Android.Support.v4.dll", assembly_data_Xamarin_Android_Support_v4_dll, 1973760}, 448906};
extern const unsigned char assembly_data_Xamarin_Android_Support_v7_AppCompat_dll [];
static CompressedAssembly assembly_bundle_Xamarin_Android_Support_v7_AppCompat_dll = {{"Xamarin.Android.Support.v7.AppCompat.dll", assembly_data_Xamarin_Android_Support_v7_AppCompat_dll, 888832}, 205477};
extern const unsigned char assembly_data_Xamarin_Android_Support_v7_RecyclerView_dll [];
static CompressedAssembly assembly_bundle_Xamarin_Android_Support_v7_RecyclerView_dll = {{"Xamarin.Android.Support.v7.RecyclerView.dll", assembly_data_Xamarin_Android_Support_v7_RecyclerView_dll, 556544}, 121529};
extern const unsigned char assembly_data_ZXing_Net_Mobile_Core_dll [];
static CompressedAssembly assembly_bundle_ZXing_Net_Mobile_Core_dll = {{"ZXing.Net.Mobile.Core.dll", assembly_data_ZXing_Net_Mobile_Core_dll, 12800}, 5146};
extern const unsigned char assembly_data_zxing_portable_dll [];
static CompressedAssembly assembly_bundle_zxing_portable_dll = {{"zxing.portable.dll", assembly_data_zxing_portable_dll, 444416}, 202887};
extern const unsigned char assembly_data_ZXingNetMobile_dll [];
static CompressedAssembly assembly_bundle_ZXingNetMobile_dll = {{"ZXingNetMobile.dll", assembly_data_ZXingNetMobile_dll, 35328}, 16603};
extern const unsigned char assembly_data_System_ServiceModel_Internals_dll [];
static CompressedAssembly assembly_bundle_System_ServiceModel_Internals_dll = {{"System.ServiceModel.Internals.dll", assembly_data_System_ServiceModel_Internals_dll, 227840}, 86213};
extern const unsigned char assembly_data_Mono_Android_dll [];
static CompressedAssembly assembly_bundle_Mono_Android_dll = {{"Mono.Android.dll", assembly_data_Mono_Android_dll, 1250816}, 316840};
extern const unsigned char assembly_data_Mono_Data_Sqlite_dll [];
static CompressedAssembly assembly_bundle_Mono_Data_Sqlite_dll = {{"Mono.Data.Sqlite.dll", assembly_data_Mono_Data_Sqlite_dll, 2078208}, 2008445};
extern const unsigned char assembly_config_Mono_Data_Sqlite_dll [];
extern const unsigned char assembly_data_mscorlib_dll [];
static CompressedAssembly assembly_bundle_mscorlib_dll = {{"mscorlib.dll", assembly_data_mscorlib_dll, 2013184}, 682072};
extern const unsigned char assembly_data_System_Core_dll [];
static CompressedAssembly assembly_bundle_System_Core_dll = {{"System.Core.dll", assembly_data_System_Core_dll, 40448}, 19185};
extern const unsigned char assembly_data_System_Data_dll [];
static CompressedAssembly assembly_bundle_System_Data_dll = {{"System.Data.dll", assembly_data_System_Data_dll, 646144}, 240260};
extern const unsigned char assembly_data_System_dll [];
static CompressedAssembly assembly_bundle_System_dll = {{"System.dll", assembly_data_System_dll, 739840}, 284536};
extern const unsigned char assembly_data_System_Transactions_dll [];
static CompressedAssembly assembly_bundle_System_Transactions_dll = {{"System.Transactions.dll", assembly_data_System_Transactions_dll, 14336}, 6129};
extern const unsigned char assembly_data_System_Xml_dll [];
static CompressedAssembly assembly_bundle_System_Xml_dll = {{"System.Xml.dll", assembly_data_System_Xml_dll, 1010176}, 357954};
extern const unsigned char assembly_data_System_Runtime_Serialization_dll [];
static CompressedAssembly assembly_bundle_System_Runtime_Serialization_dll = {{"System.Runtime.Serialization.dll", assembly_data_System_Runtime_Serialization_dll, 6144}, 2219};
extern const unsigned char assembly_data_System_Numerics_dll [];
static CompressedAssembly assembly_bundle_System_Numerics_dll = {{"System.Numerics.dll", assembly_data_System_Numerics_dll, 23040}, 10512};

static const CompressedAssembly *compressed [] = {
	&assembly_bundle_POSV1_dll,
	&assembly_bundle_Java_Interop_dll,
	&assembly_bundle_Refractored_FloatingActionButton_dll,
	&assembly_bundle_System_Data_Portable_dll,
	&assembly_bundle_System_Transactions_Portable_dll,
	&assembly_bundle_Xamarin_Android_Support_Design_dll,
	&assembly_bundle_Xamarin_Android_Support_v4_dll,
	&assembly_bundle_Xamarin_Android_Support_v7_AppCompat_dll,
	&assembly_bundle_Xamarin_Android_Support_v7_RecyclerView_dll,
	&assembly_bundle_ZXing_Net_Mobile_Core_dll,
	&assembly_bundle_zxing_portable_dll,
	&assembly_bundle_ZXingNetMobile_dll,
	&assembly_bundle_System_ServiceModel_Internals_dll,
	&assembly_bundle_Mono_Android_dll,
	&assembly_bundle_Mono_Data_Sqlite_dll,
	&assembly_bundle_mscorlib_dll,
	&assembly_bundle_System_Core_dll,
	&assembly_bundle_System_Data_dll,
	&assembly_bundle_System_dll,
	&assembly_bundle_System_Transactions_dll,
	&assembly_bundle_System_Xml_dll,
	&assembly_bundle_System_Runtime_Serialization_dll,
	&assembly_bundle_System_Numerics_dll,
	NULL
};

static char *image_name = "POSV1.dll";

static void install_dll_config_files (void (register_config_for_assembly_func)(const char *, const char *)) {

	register_config_for_assembly_func ("Mono.Data.Sqlite.dll", assembly_config_Mono_Data_Sqlite_dll);

}

static const char *config_dir = NULL;
static MonoBundledAssembly **bundled;

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <zlib.h>

static int
my_inflate (const Byte *compr, uLong compr_len, Byte *uncompr, uLong uncompr_len)
{
	int err;
	z_stream stream;

	memset (&stream, 0, sizeof (z_stream));
	stream.next_in = (Byte *) compr;
	stream.avail_in = (uInt) compr_len;

	// http://www.zlib.net/manual.html
	err = inflateInit2 (&stream, 16+MAX_WBITS);
	if (err != Z_OK)
		return 1;

	for (;;) {
		stream.next_out = uncompr;
		stream.avail_out = (uInt) uncompr_len;
		err = inflate (&stream, Z_NO_FLUSH);
		if (err == Z_STREAM_END) break;
		if (err != Z_OK) {
			printf ("%d\n", err);
			return 2;
		}
	}

	err = inflateEnd (&stream);
	if (err != Z_OK)
		return 3;

	if (stream.total_out != uncompr_len)
		return 4;
	
	return 0;
}

void mono_mkbundle_init (void (register_bundled_assemblies_func)(const MonoBundledAssembly **), void (register_config_for_assembly_func)(const char *, const char *))
{
	CompressedAssembly **ptr;
	MonoBundledAssembly **bundled_ptr;
	Bytef *buffer;
	int nbundles;

	install_dll_config_files (register_config_for_assembly_func);

	ptr = (CompressedAssembly **) compressed;
	nbundles = 0;
	while (*ptr++ != NULL)
		nbundles++;

	bundled = (MonoBundledAssembly **) malloc (sizeof (MonoBundledAssembly *) * (nbundles + 1));
	bundled_ptr = bundled;
	ptr = (CompressedAssembly **) compressed;
	while (*ptr != NULL) {
		uLong real_size;
		uLongf zsize;
		int result;
		MonoBundledAssembly *current;

		real_size = (*ptr)->assembly.size;
		zsize = (*ptr)->compressed_size;
		buffer = (Bytef *) malloc (real_size);
		result = my_inflate ((*ptr)->assembly.data, zsize, buffer, real_size);
		if (result != 0) {
			fprintf (stderr, "mkbundle: Error %d decompressing data for %s\n", result, (*ptr)->assembly.name);
			exit (1);
		}
		(*ptr)->assembly.data = buffer;
		current = (MonoBundledAssembly *) malloc (sizeof (MonoBundledAssembly));
		memcpy (current, *ptr, sizeof (MonoBundledAssembly));
		current->name = (*ptr)->assembly.name;
		*bundled_ptr = current;
		bundled_ptr++;
		ptr++;
	}
	*bundled_ptr = NULL;
	register_bundled_assemblies_func((const MonoBundledAssembly **) bundled);
}
