#define JEMALLOC_H_
extern "C" {


/* Defined if __attribute__((...)) syntax is supported. */
/* #undef JEMALLOC_HAVE_ATTR */

/* Defined if alloc_size attribute is supported. */
/* #undef JEMALLOC_HAVE_ATTR_ALLOC_SIZE */

/* Defined if format(gnu_printf, ...) attribute is supported. */
/* #undef JEMALLOC_HAVE_ATTR_FORMAT_GNU_PRINTF */

/* Defined if format(printf, ...) attribute is supported. */
/* #undef JEMALLOC_HAVE_ATTR_FORMAT_PRINTF */

/*
 * Define overrides for non-standard allocator-related functions if they are
 * present on the system.
 */
/* #undef JEMALLOC_OVERRIDE_MEMALIGN */
/* #undef JEMALLOC_OVERRIDE_VALLOC */

/*
 * At least Linux omits the "const" in:
 *
 *   size_t malloc_usable_size(const void *ptr);
 *
 * Match the operating system's prototype.
 */
#define JEMALLOC_USABLE_SIZE_CONST const

/*
 * If defined, specify throw() for the public function prototypes when compiling
 * with C++.  The only justification for this is to match the prototypes that
 * glibc defines.
 */
/* #undef JEMALLOC_USE_CXX_THROW */

#define LG_SIZEOF_PTR_WIN 3

/* sizeof(void *) == 2^LG_SIZEOF_PTR. */
#define LG_SIZEOF_PTR LG_SIZEOF_PTR_WIN

/*
 * Name mangling for public symbols is controlled by --with-mangling and
 * --with-jemalloc-prefix.  With default settings the je_ prefix is stripped by
 * these macro definitions.
 */

#  define je_aligned_alloc je_aligned_alloc
#  define je_calloc je_calloc
#  define je_dallocx je_dallocx
#  define je_free je_free
#  define je_mallctl je_mallctl
#  define je_mallctlbymib je_mallctlbymib
#  define je_mallctlnametomib je_mallctlnametomib
#  define je_malloc je_malloc
#  define je_malloc_conf je_malloc_conf
#  define je_malloc_message je_malloc_message
#  define je_malloc_stats_print je_malloc_stats_print
#  define je_malloc_usable_size je_malloc_usable_size
#  define je_mallocx je_mallocx
#  define je_nallocx je_nallocx
#  define je_posix_memalign je_posix_memalign
#  define je_rallocx je_rallocx
#  define je_realloc je_realloc
#  define je_sallocx je_sallocx
#  define je_sdallocx je_sdallocx
#  define je_xallocx je_xallocx


/*
#include <stdlib.h>
#include <stdbool.h>
#include <stdint.h>
#include <limits.h>
#include <strings.h>
*/

#define JEMALLOC_VERSION "5.0.1-64-gcb3b72b9756d124565ed12e005065ad6f0769568"
#define JEMALLOC_VERSION_MAJOR 5
#define JEMALLOC_VERSION_MINOR 0
#define JEMALLOC_VERSION_BUGFIX 1
#define JEMALLOC_VERSION_NREV 64
#define JEMALLOC_VERSION_GID "cb3b72b9756d124565ed12e005065ad6f0769568"

#define MALLOCX_LG_ALIGN(la)	((int)(la))
#define MALLOCX_ALIGN(a)						\
     ((int)(((size_t)(a) < (size_t)INT_MAX) ? ffs((int)(a))-1 :	\
     ffs((int)(((size_t)(a))>>32))+31))

#define MALLOCX_ZERO	((int)0x40)
/*
 * Bias tcache index bits so that 0 encodes "automatic tcache management", and 1
 * encodes MALLOCX_TCACHE_NONE.
 */
#define MALLOCX_TCACHE(tc)	((int)(((tc)+2) << 8))
#define MALLOCX_TCACHE_NONE	MALLOCX_TCACHE(-1)
/*
 * Bias arena index bits so that 0 encodes "use an automatically chosen arena".
 */
#define MALLOCX_ARENA(a)	((((int)(a))+1) << 20)

/*
 * Use as arena index in "arena.<i>.{purge,decay,dss}" and
 * "stats.arenas.<i>.*" mallctl interfaces to select all arenas.  This
 * definition is intentionally specified in raw decimal format to support
 * cpp-based string concatenation, e.g.
 *
 *   #define STRINGIFY_HELPER(x) #x
 *   #define STRINGIFY(x) STRINGIFY_HELPER(x)
 *
 *   mallctl("arena." STRINGIFY(MALLCTL_ARENAS_ALL) ".purge", NULL, NULL, NULL,
 *       0);
 */
#define MALLCTL_ARENAS_ALL	4096
/*
 * Use as arena index in "stats.arenas.<i>.*" mallctl interfaces to select
 * destroyed arenas.
 */
#define MALLCTL_ARENAS_DESTROYED	4097

#if defined(__cplusplus) && defined(JEMALLOC_USE_CXX_THROW)
#  define JEMALLOC_CXX_THROW throw()
#else
#  define JEMALLOC_CXX_THROW
#endif


#  define JEMALLOC_ATTR(s)
#  define JEMALLOC_ALIGNED(s) __declspec(align(s))
#  define JEMALLOC_ALLOC_SIZE(s)
#  define JEMALLOC_ALLOC_SIZE2(s1, s2)
#  define JEMALLOC_EXPORT __declspec(dllexport)
#  define JEMALLOC_FORMAT_PRINTF(s, i)
#  define JEMALLOC_NOINLINE __declspec(noinline)
#  define JEMALLOC_NOTHROW
#  define JEMALLOC_SECTION(s) __declspec(allocate(s))
#  define JEMALLOC_RESTRICT_RETURN __declspec(restrict)
#  define JEMALLOC_ALLOCATOR

/*
 * The je_ prefix on the following public symbol declarations is an artifact
 * of namespace management, and should be omitted in application code unless
 * JEMALLOC_NO_DEMANGLE is defined (see jemalloc_mangle.h).
 */
extern JEMALLOC_EXPORT const char	*je_malloc_conf;
extern JEMALLOC_EXPORT void		(*je_malloc_message)(void *cbopaque, const char *s);


JEMALLOC_EXPORT JEMALLOC_ALLOCATOR JEMALLOC_RESTRICT_RETURN
    void JEMALLOC_NOTHROW	*je_malloc(size_t size)
    JEMALLOC_CXX_THROW JEMALLOC_ATTR(malloc) JEMALLOC_ALLOC_SIZE(1);
JEMALLOC_EXPORT JEMALLOC_ALLOCATOR JEMALLOC_RESTRICT_RETURN
    void JEMALLOC_NOTHROW	*je_calloc(size_t num, size_t size)
    JEMALLOC_CXX_THROW JEMALLOC_ATTR(malloc) JEMALLOC_ALLOC_SIZE2(1, 2);
JEMALLOC_EXPORT int JEMALLOC_NOTHROW	je_posix_memalign(void **memptr,
    size_t alignment, size_t size) JEMALLOC_CXX_THROW JEMALLOC_ATTR(nonnull(1));
JEMALLOC_EXPORT JEMALLOC_ALLOCATOR JEMALLOC_RESTRICT_RETURN
    void JEMALLOC_NOTHROW	*je_aligned_alloc(size_t alignment,
    size_t size) JEMALLOC_CXX_THROW JEMALLOC_ATTR(malloc)
    JEMALLOC_ALLOC_SIZE(2);
JEMALLOC_EXPORT JEMALLOC_ALLOCATOR JEMALLOC_RESTRICT_RETURN
    void JEMALLOC_NOTHROW	*je_realloc(void *ptr, size_t size)
    JEMALLOC_CXX_THROW JEMALLOC_ALLOC_SIZE(2);
JEMALLOC_EXPORT void JEMALLOC_NOTHROW	je_free(void *ptr)
    JEMALLOC_CXX_THROW;

JEMALLOC_EXPORT JEMALLOC_ALLOCATOR JEMALLOC_RESTRICT_RETURN
    void JEMALLOC_NOTHROW	*je_mallocx(size_t size, int flags)
    JEMALLOC_ATTR(malloc) JEMALLOC_ALLOC_SIZE(1);
JEMALLOC_EXPORT JEMALLOC_ALLOCATOR JEMALLOC_RESTRICT_RETURN
    void JEMALLOC_NOTHROW	*je_rallocx(void *ptr, size_t size,
    int flags) JEMALLOC_ALLOC_SIZE(2);
JEMALLOC_EXPORT size_t JEMALLOC_NOTHROW	je_xallocx(void *ptr, size_t size,
    size_t extra, int flags);
JEMALLOC_EXPORT size_t JEMALLOC_NOTHROW	je_sallocx(const void *ptr,
    int flags) JEMALLOC_ATTR(pure);
JEMALLOC_EXPORT void JEMALLOC_NOTHROW	je_dallocx(void *ptr, int flags);
JEMALLOC_EXPORT void JEMALLOC_NOTHROW	je_sdallocx(void *ptr, size_t size,
    int flags);
JEMALLOC_EXPORT size_t JEMALLOC_NOTHROW	je_nallocx(size_t size, int flags)
    JEMALLOC_ATTR(pure);

JEMALLOC_EXPORT int JEMALLOC_NOTHROW	je_mallctl(const char *name,
    void *oldp, size_t *oldlenp, void *newp, size_t newlen);
JEMALLOC_EXPORT int JEMALLOC_NOTHROW	je_mallctlnametomib(const char *name,
    size_t *mibp, size_t *miblenp);
JEMALLOC_EXPORT int JEMALLOC_NOTHROW	je_mallctlbymib(const size_t *mib,
    size_t miblen, void *oldp, size_t *oldlenp, void *newp, size_t newlen);
JEMALLOC_EXPORT void JEMALLOC_NOTHROW	je_malloc_stats_print(
    void (*write_cb)(void *, const char *), void *je_cbopaque,
    const char *opts);
JEMALLOC_EXPORT size_t JEMALLOC_NOTHROW	je_malloc_usable_size(
    JEMALLOC_USABLE_SIZE_CONST void *ptr) JEMALLOC_CXX_THROW;

JEMALLOC_EXPORT void JEMALLOC_NOTHROW je_set_malloc_conf(const char *name)
JEMALLOC_EXPORT char* JEMALLOC_NOTHROW je_get_malloc_conf()

typedef struct extent_hooks_s extent_hooks_t;

/*
 * void *
 * extent_alloc(extent_hooks_t *extent_hooks, void *new_addr, size_t size,
 *     size_t alignment, bool *zero, bool *commit, unsigned arena_ind);
 */
typedef void *(extent_alloc_t)(extent_hooks_t *, void *, size_t, size_t, bool *,
    bool *, unsigned);

/*
 * bool
 * extent_dalloc(extent_hooks_t *extent_hooks, void *addr, size_t size,
 *     bool committed, unsigned arena_ind);
 */
typedef bool (extent_dalloc_t)(extent_hooks_t *, void *, size_t, bool,
    unsigned);

/*
 * void
 * extent_destroy(extent_hooks_t *extent_hooks, void *addr, size_t size,
 *     bool committed, unsigned arena_ind);
 */
typedef void (extent_destroy_t)(extent_hooks_t *, void *, size_t, bool,
    unsigned);

/*
 * bool
 * extent_commit(extent_hooks_t *extent_hooks, void *addr, size_t size,
 *     size_t offset, size_t length, unsigned arena_ind);
 */
typedef bool (extent_commit_t)(extent_hooks_t *, void *, size_t, size_t, size_t,
    unsigned);

/*
 * bool
 * extent_decommit(extent_hooks_t *extent_hooks, void *addr, size_t size,
 *     size_t offset, size_t length, unsigned arena_ind);
 */
typedef bool (extent_decommit_t)(extent_hooks_t *, void *, size_t, size_t,
    size_t, unsigned);

/*
 * bool
 * extent_purge(extent_hooks_t *extent_hooks, void *addr, size_t size,
 *     size_t offset, size_t length, unsigned arena_ind);
 */
typedef bool (extent_purge_t)(extent_hooks_t *, void *, size_t, size_t, size_t,
    unsigned);

/*
 * bool
 * extent_split(extent_hooks_t *extent_hooks, void *addr, size_t size,
 *     size_t size_a, size_t size_b, bool committed, unsigned arena_ind);
 */
typedef bool (extent_split_t)(extent_hooks_t *, void *, size_t, size_t, size_t,
    bool, unsigned);

/*
 * bool
 * extent_merge(extent_hooks_t *extent_hooks, void *addr_a, size_t size_a,
 *     void *addr_b, size_t size_b, bool committed, unsigned arena_ind);
 */
typedef bool (extent_merge_t)(extent_hooks_t *, void *, size_t, void *, size_t,
    bool, unsigned);

struct extent_hooks_s {
	extent_alloc_t		*alloc;
	extent_dalloc_t		*dalloc;
	extent_destroy_t	*destroy;
	extent_commit_t		*commit;
	extent_decommit_t	*decommit;
	extent_purge_t		*purge_lazy;
	extent_purge_t		*purge_forced;
	extent_split_t		*split;
	extent_merge_t		*merge;
};


}

