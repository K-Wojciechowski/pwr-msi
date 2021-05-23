using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pwr_msi.Services;

namespace pwr_msi.Controllers {
    [Controller]
    [Route("uploads/")]
    public class UploadsAccessController {
        private readonly S3Service _s3Service;

        public UploadsAccessController(S3Service s3Service) {
            _s3Service = s3Service;
        }

        [Route("{rid}/{fileBaseName}")]
        public async Task<FileStreamResult> GetUploadedFile([FromRoute] int rid, [FromRoute] string fileBaseName) {
            var fileName = $"{rid}/{fileBaseName}";
            return await _s3Service.GetFileAsResponse(fileName);
        }
    }
}
